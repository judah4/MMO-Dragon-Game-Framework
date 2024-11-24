using Lidgren.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mmogf.Core.Contracts;
using Mmogf.Core.Contracts.Commands;
using Mmogf.Core.Contracts.Events;
using Mmogf.Servers;
using Mmogf.Servers.Serializers;
using Mmogf.Servers.Shared;
using Prometheus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MmoGameFramework
{
    public sealed class MmoServer : IServerTransportation
    {
        public bool Active => s_server.Status == NetPeerStatus.Running;
        public string WorkerType => _config.AppIdentifier;

        private readonly Gauge ConnectedWorkersGauge;

        private NetServer s_server;
        private EntityStore _entities;
        public EntityStore Entities => _entities;
        NetPeerConfiguration _config;
        ILogger _logger;
        IConfiguration _configuration;
        private ISerializer _serializer;
        private EntityToContractConverter _entityToContractConverter;

        bool _clientWorker = false;
        Stopwatch _stopwatch;
        int _tickRate;
        public int TickRate => _tickRate;

        public Dictionary<RemoteWorkerIdentifier, LidgrenWorkerConnection> _connections = new Dictionary<RemoteWorkerIdentifier, LidgrenWorkerConnection>();
        public ConcurrentDictionary<RemoteWorkerIdentifier, LidgrenWorkerConnection> _workerWithSubChanges = new ConcurrentDictionary<RemoteWorkerIdentifier, LidgrenWorkerConnection>();

        public MmoServer(EntityStore entities, NetPeerConfiguration config, bool clientWorker, ISerializer serializer, ILogger<MmoServer> logger, IConfiguration configuration, EntityToContractConverter entityToContractConverter)
        {
            _entities = entities;
            _clientWorker = clientWorker;
            _serializer = serializer;
            _configuration = configuration;
            _tickRate = _configuration.GetValue<int?>(key: "TickRate") ?? 60;
            var description = "Number of connected workers.";
            if (_clientWorker)
            {
                description = "Number of connected clients.";
            }
            ConnectedWorkersGauge = Metrics.CreateGauge($"dragongf_{config.AppIdentifier.Replace('-', '_')}", description);
            _stopwatch = new Stopwatch();

            // set up network
            _config = config;
            s_server = new NetServer(_config);
            _logger = logger;

            _entities.OnUpdateEntityFull += OnEntityUpdateFull;
            _entities.OnEntityDelete += OnEntityDelete;
            _entities.OnEntityEvent += OnEntityEvent;
            _entities.OnEntityCommand += OnEntityCommand;
            _entities.OnEntityCommandResponse += OnEntityCommandResponse;
            _entities.OnUpdateEntityPartial += OnEntityUpdatePartial;

            _entities.OnEntityAddSubscription += ProcessOnEntityAddSubscription;
            _entities.OnEntityRemoveSubscription += ProcessOnEntityRemoveSubscription;
            _entityToContractConverter = entityToContractConverter;
        }

        public void Start()
        {
            s_server.Start();

            var thread1 = new Thread(async () => await Loop());
            thread1.Priority = ThreadPriority.AboveNormal;
            thread1.Start();

        }

        private void MessageCallback(object state)
        {
            NetIncomingMessage im = s_server.ReadMessage();

            // Note: This should never happen but I'm not going to bet it won't.
            if (im == null)
            {
                _logger.LogError("Callback with no Message");
                return;
            }

            var workerId = new RemoteWorkerIdentifier(im.SenderConnection.RemoteUniqueIdentifier);

            // handle incoming message
            switch (im.MessageType)
            {
                case NetIncomingMessageType.DebugMessage:
                    string text = im.ReadString();
                    _logger.LogDebug(text);
                    break;
                case NetIncomingMessageType.ErrorMessage:
                    string text2 = im.ReadString();
                    _logger.LogError(text2);
                    break;
                case NetIncomingMessageType.WarningMessage:
                    string text3 = im.ReadString();
                    _logger.LogWarning(text3);
                    break;
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text4 = im.ReadString();
                    _logger.LogDebug(text4);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                    string reason = im.ReadString();
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug(im.SenderConnection.RemoteUniqueIdentifier + " " + status + ": " + reason);

                    if (status == NetConnectionStatus.Connected)
                    {
                        HandleWorkerConnect(im);
                    }
                    else if (status == NetConnectionStatus.Disconnected)
                    {
                        _logger.LogInformation($"{_config.AppIdentifier} {im.SenderConnection.RemoteUniqueIdentifier} Disconnected");
                        LidgrenWorkerConnection worker;
                        if (_connections.TryGetValue(workerId, out worker))
                            HandleWorkerDisconnect(im, worker);
                    }

                    break;
                case NetIncomingMessageType.Data:
                    //if(_logger.IsEnabled(LogLevel.Debug))
                    //    _logger.LogDebug(im.SenderConnection.RemoteUniqueIdentifier +" - '" + BitConverter.ToString(im.Data) + "'");
                    var simpleData = _serializer.Deserialize<MmoMessage>(im.Data);
                    //if (_logger.IsEnabled(LogLevel.Debug))
                    //    _logger.LogDebug($"Serve Codes {simpleData.MessageId}");
                    switch (simpleData.MessageId)
                    {
                        case ServerCodes.ChangeInterestArea:
                            var interestArea = _serializer.Deserialize<ChangeInterestArea>(simpleData.Info);
                            LidgrenWorkerConnection worker;
                            if (_connections.TryGetValue(workerId, out worker))
                            {
                                worker.InterestPosition = interestArea.Position.ToPosition();

                                var results = _entities.UpdateWorkerInterestArea(worker);
                                foreach (var add in results.addEntityIds)
                                    HandleEntitySubChange(worker, true, add);
                                foreach (var add in results.removeEntityIds)
                                    HandleEntitySubChange(worker, false, add);
                                SendToWorker(worker, new MmoMessage()
                                {
                                    MessageId = ServerCodes.EntityCheckout,
                                    Info = _serializer.Serialize(new EntityCheckout()
                                    {
                                        Checkouts = new List<EntityId>(),
                                        Remove = false,
                                    }),
                                }, NetDeliveryMethod.ReliableOrdered);
                            }
                            break;
                        case ServerCodes.EntityUpdate:
                            HandleEntityUpdate(im, simpleData);
                            break;
                        case ServerCodes.EntityEvent:
                            HandleEntityEvent(im, simpleData);
                            break;
                        case ServerCodes.EntityCommandRequest:
                            HandleEntityCommand(im, simpleData);
                            break;
                        case ServerCodes.EntityCommandResponse:
                            HandleEntityCommandResponse(im, simpleData);
                            break;
                        case ServerCodes.Ping:
                            if (_connections.TryGetValue(workerId, out worker))
                            {
                                SendToWorker(worker, new MmoMessage()
                                {
                                    MessageId = ServerCodes.Ping,
                                    Info = new byte[0],
                                });
                            }
                            break;
                        default:
                            // incoming chat message from a client
                            //string chat = im.ReadString();

                            break;
                    }

                    break;
                default:
                    _logger.LogError("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                    break;
            }

            s_server.Recycle(im);

        }

        public void Stop()
        {
            //send stop to peers
            s_server.Shutdown("End");
        }

        private async Task Loop()
        {
            // No idea what this does but Lidgren needs it to be happy
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            // Register a Callback for Testing
            s_server.RegisterReceivedCallback(MessageCallback, SynchronizationContext.Current);

            while (s_server.Status != NetPeerStatus.NotRunning)
            {
                _stopwatch.Restart();
                try
                {

                    HandleEntitySubChanges();

                    ConnectedWorkersGauge.Set(s_server.ConnectionsCount);

                    var time = _stopwatch.ElapsedMilliseconds;

                    var tickMilliseconds = 1000.0 / _tickRate;

                    if (time < tickMilliseconds)
                    {
                        int delay = (int)(tickMilliseconds - time);
                        if (delay > 0)
                        {
                            await Task.Delay(delay);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
        }

        /// <summary>
        /// Handle interest area changes once per tick
        /// </summary>
        private void HandleEntitySubChanges()
        {
            foreach (var workerPair in _workerWithSubChanges)
            {
                var worker = workerPair.Value;
                if (_logger.IsEnabled(LogLevel.Debug) && worker.EntitiesToAdd.Count > 0)
                    _logger.LogDebug($"Adding Entities ({string.Join(',', worker.EntitiesToAdd)}) to Worker {worker.ConnectionType}-{worker.WorkerId}");
                foreach (var add in worker.EntitiesToAdd)
                {
                    var entity = _entities.GetEntity(add.Key);
                    if (entity == null)
                        continue;

                    var message = EntityInfoMessage(entity);

                    SendToWorker(worker, message, NetDeliveryMethod.ReliableUnordered);
                }
                if (_logger.IsEnabled(LogLevel.Debug) && worker.EntitiesToRemove.Count > 0)
                    _logger.LogDebug($"Removing Entities ({string.Join(',', worker.EntitiesToRemove)}) from Worker {worker.ConnectionType}-{worker.WorkerId}");
                foreach (var remove in worker.EntitiesToRemove)
                {
                    var message = EntityDeleteMessage(remove.Key);
                    SendToWorker(worker, message, NetDeliveryMethod.ReliableUnordered);
                }
                worker.EntitiesToAdd.Clear();
                worker.EntitiesToRemove.Clear();
            }
            _workerWithSubChanges.Clear();
        }

        private void HandleWorkerDisconnect(NetIncomingMessage im, LidgrenWorkerConnection worker)
        {
            if (worker == null)
                return;

            _entities.RemoveWorker(worker);

            _connections.Remove(worker.WorkerId);

        }

        private void HandleWorkerConnect(NetIncomingMessage im)
        {
            var interestRange = 2000;
            if (_clientWorker)
            {
                interestRange = 100;
            }
            //todo: do some sort of worker type validation from a config
            var workerConnection = new LidgrenWorkerConnection(im.SenderConnection.RemoteHailMessage.ReadString(), im.SenderConnection, Position.Zero, interestRange);
            _connections.Add(workerConnection.WorkerId, workerConnection);
            var results = _entities.UpdateWorkerInterestArea(workerConnection);
            foreach (var add in results.addEntityIds)
                HandleEntitySubChange(workerConnection, true, add);
            foreach (var add in results.removeEntityIds)
                HandleEntitySubChange(workerConnection, false, add);

            _logger.LogInformation("Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString());
            var message = new MmoMessage()
            {
                MessageId = ServerCodes.ClientConnect,
                Info = _serializer.Serialize(new ClientConnect()
                {
                    ClientId = im.SenderConnection.RemoteUniqueIdentifier,
                }),
            };
            NetOutgoingMessage om = s_server.CreateMessage();
            om.Write(_serializer.Serialize(message));
            s_server.SendMessage(om, im.SenderConnection, NetDeliveryMethod.ReliableUnordered);
        }

        private void HandleEntityCommand(NetIncomingMessage im, MmoMessage simpleData)
        {
            //get command info
            var commandRequest = _serializer.Deserialize<CommandRequest>(simpleData.Info);

            if (!commandRequest.Header.EntityId.IsValid() && commandRequest.Header.ComponentId == 0)
            {
                //world command
                HandleWorldCommand(im, simpleData, commandRequest);
                return;
            }

            var entityInfo = _entities.GetEntity(commandRequest.Header.EntityId);

            if (entityInfo == null)
                return;

            var workerId = new RemoteWorkerIdentifier(im.SenderConnection.RemoteUniqueIdentifier);
            LidgrenWorkerConnection worker;
            if (!_connections.TryGetValue(workerId, out worker))
            {
                //send failure
                SendToWorker(worker, new MmoMessage()
                {
                    MessageId = ServerCodes.EntityCommandResponse,
                    Info = _serializer.Serialize(new CommandResponse(CommandResponseHeader.Create(commandRequest.Header, CommandStatus.InvalidRequest, "No Worker Identified"), null)),
                });
                return;
            }

            var modifiedHeader = commandRequest.Header;
            modifiedHeader.RequestorWorkerType = worker.ConnectionType;
            modifiedHeader.RequesterId = worker.Connection.RemoteUniqueIdentifier;
            commandRequest.Header = modifiedHeader;

            // pass to authority aka worker

            _entities.SendCommand(commandRequest);

        }

        private void HandleWorldCommand(NetIncomingMessage im, MmoMessage message, CommandRequest commandRequest)
        {
            var workerId = new RemoteWorkerIdentifier(im.SenderConnection.RemoteUniqueIdentifier);
            LidgrenWorkerConnection worker;
            if (!_connections.TryGetValue(workerId, out worker))
            {
                //send failure
                SendToWorker(worker, new MmoMessage()
                {
                    MessageId = ServerCodes.EntityCommandResponse,
                    Info = _serializer.Serialize(new CommandResponse(CommandResponseHeader.Create(commandRequest.Header, CommandStatus.InvalidRequest, "No Worker Identified"), null)),
                });
                return;
            }

            //todo: verify sender has permissions from config settings

            if (worker.ConnectionType != "Dragon-Worker")
            {
                SendToWorker(worker, new MmoMessage()
                {
                    MessageId = ServerCodes.EntityCommandResponse,
                    Info = _serializer.Serialize(new CommandResponse(CommandResponseHeader.Create(commandRequest.Header, CommandStatus.InvalidRequest, "No permission to create entities."), null)),
                });
                return;
            }

            switch (commandRequest.Header.CommandId)
            {
                case World.CreateEntity.CommandId:
                    var createEntity = _serializer.Deserialize<World.CreateEntity>(commandRequest.Payload);

                    var requestPayload = createEntity.Request.Value;
                    var entityInfo = _entities.Create(requestPayload.EntityType, requestPayload.Position.ToPosition(), requestPayload.Rotation, requestPayload.Acls, null, requestPayload.Components);
                    _entities.UpdateEntity(entityInfo);
                    createEntity.Response = new NothingInternal();
                    SendToWorker(worker, new MmoMessage()
                    {
                        MessageId = ServerCodes.EntityCommandResponse,
                        Info = _serializer.Serialize(new CommandResponse(CommandResponseHeader.Create(commandRequest.Header, CommandStatus.Success, ""), _serializer.Serialize(createEntity))),
                    }, NetDeliveryMethod.ReliableUnordered);
                    break;
                case World.DeleteEntity.CommandId:
                    var deleteEntity = _serializer.Deserialize<World.DeleteEntity>(commandRequest.Payload);
                    var deleteRequestPayload = deleteEntity.Request.Value;
                    _entities.Delete(deleteRequestPayload.EntityId);
                    deleteEntity.Response = new NothingInternal();
                    SendToWorker(worker, new MmoMessage()
                    {
                        MessageId = ServerCodes.EntityCommandResponse,
                        Info = _serializer.Serialize(new CommandResponse(CommandResponseHeader.Create(commandRequest.Header, CommandStatus.Success, ""), _serializer.Serialize(deleteEntity))),
                    }, NetDeliveryMethod.ReliableUnordered);
                    break;
            }
        }

        private void HandleEntityCommandResponse(NetIncomingMessage im, MmoMessage simpleData)
        {
            //get command info
            var commandResponse = _serializer.Deserialize<CommandResponse>(simpleData.Info);
            var entityInfo = _entities.GetEntity(commandResponse.Header.EntityId);

            if (entityInfo == null)
                return;

            //pass to authority aka worker

            _entities.SendCommandResponse(commandResponse);

        }

        private void HandleEntityUpdate(NetIncomingMessage im, MmoMessage simpleData)
        {
            var entityUpdate = _serializer.Deserialize<EntityUpdate>(simpleData.Info);
            var entityVal = _entities.GetEntity(entityUpdate.EntityId);

            if (entityVal == null)
                return;

            Entity entity = entityVal;

            var workerId = new RemoteWorkerIdentifier(im.SenderConnection.RemoteUniqueIdentifier);
            LidgrenWorkerConnection worker;
            if (!_connections.TryGetValue(workerId, out worker))
            {
                //disconnected??
                return;
            }
            var acls = entity.Acls;
            if (!acls.CanWrite(entityUpdate.ComponentId, worker.ConnectionType))
            {
                //log
                return;
            }

            var componentData = new EntityComponentData(entityUpdate.Info);
            entity.UpdateComponent(entityUpdate.ComponentId, componentData);
            //if (_logger.IsEnabled(LogLevel.Debug) && entityUpdate.ComponentId == Position.ComponentId)
            //{
            //    var position = _serializer.Deserialize<Position>(entityUpdate.Info);
            //    _logger.LogDebug($"Entity: {entityInfo.Value.EntityId} position to {position.ToString()}");
            //}

            _entities.UpdateEntityPartial(entity, entityUpdate, new RemoteWorkerIdentifier(worker.Connection.RemoteUniqueIdentifier));

        }

        private void HandleEntityEvent(NetIncomingMessage im, MmoMessage simpleData)
        {
            var eventRequest = _serializer.Deserialize<EventRequest>(simpleData.Info);
            var entityInfo = _entities.GetEntity(eventRequest.Header.EntityId);

            if (entityInfo == null)
                return;
            var workerId = new RemoteWorkerIdentifier(im.SenderConnection.RemoteUniqueIdentifier);
            LidgrenWorkerConnection worker;
            if (!_connections.TryGetValue(workerId, out worker))
            {
                //disconnected??
                return;
            }
            var acls = entityInfo.Acls;
            if (!acls.CanWrite(eventRequest.Header.ComponentId, worker.ConnectionType))
            {
                //log
                return;
            }

            _entities.SendEvent(eventRequest);
        }

        public void SendToWorker(LidgrenWorkerConnection worker, MmoMessage message, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            var bytes = _serializer.Serialize(message);
            NetOutgoingMessage om = s_server.CreateMessage(bytes.Length);
            om.Write(bytes);
            s_server.SendMessage(om, worker.Connection, deliveryMethod);
        }

        private void SendToAll(MmoMessage message, NetDeliveryMethod deliveryMethod)
        {
            var bytes = _serializer.Serialize(message);
            NetOutgoingMessage om = s_server.CreateMessage(bytes.Length);
            om.Write(bytes);
            s_server.SendToAll(om, deliveryMethod);
        }

        public void SendSubscribed(Entity entity, MmoMessage message, RemoteWorkerIdentifier workerIdToExclude, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            var connections = new List<NetConnection>(10);
            var workerIds = _entities.GetSubscribedWorkers(entity);
            foreach (var workerId in workerIds)
            {
                if (workerId == workerIdToExclude)
                    continue;

                if (_connections.TryGetValue(workerId, out LidgrenWorkerConnection worker))
                    connections.Add(worker.Connection);
            }

            if (connections.Count < 1)
            {
                return;
            }

            NetOutgoingMessage om = s_server.CreateMessage();
            om.Write(_serializer.Serialize(message));
            s_server.SendMessage(om, connections, deliveryMethod, 0);
        }

        private MmoMessage EntityInfoMessage(Entity entity)
        {
            //find who has checked out
            var message = new MmoMessage()
            {
                MessageId = ServerCodes.EntityInfo,
                Info = _serializer.Serialize(_entityToContractConverter.Convert(entity)),
            };

            return message;
        }

        private void OnEntityUpdateFull(Entity entityInfo)
        {
            var message = EntityInfoMessage(entityInfo);
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Sending Entity Info {entityInfo.EntityId}");
            SendSubscribed(entityInfo, message, new RemoteWorkerIdentifier(0), NetDeliveryMethod.ReliableUnordered);
            //SendCheckedout(entityInfo.EntityId, message, NetDeliveryMethod.ReliableOrdered);
            //SendArea(entityInfo.Position, message, 0, NetDeliveryMethod.ReliableUnordered);
        }

        private MmoMessage EntityDeleteMessage(EntityId entityId)
        {
            //find who has checked out
            var message = new MmoMessage()
            {
                MessageId = ServerCodes.EntityDelete,
                Info = _serializer.Serialize(new EntityInfo()
                {
                    EntityId = entityId,
                    EntityData = new Dictionary<short, byte[]>(),
                }),
            };

            return message;
        }

        private void OnEntityDelete(Entity entityInfo)
        {
            //find who has checked out
            var message = EntityDeleteMessage(entityInfo.EntityId);

            _logger.LogInformation($"Deleting Entity {entityInfo.EntityId}");
            //SendCheckedout(entityInfo.EntityId, message, NetDeliveryMethod.ReliableOrdered);
            SendSubscribed(entityInfo, message, new RemoteWorkerIdentifier(0), NetDeliveryMethod.ReliableUnordered);
            //SendArea(entityInfo.Position, message, 0, NetDeliveryMethod.ReliableUnordered);
        }

        private void OnEntityUpdatePartial(EntityUpdate entityUpdate, RemoteWorkerIdentifier workerId)
        {

            var entity = _entities.GetEntity(entityUpdate.EntityId);
            if (entity == null)
                return;

            var message = new MmoMessage()
            {
                MessageId = ServerCodes.EntityUpdate,

                Info = _serializer.Serialize(entityUpdate),
            };
            //if (_logger.IsEnabled(LogLevel.Debug))
            //    _logger.LogDebug("Sending Entity Update");
            SendSubscribed(entity, message, workerId, NetDeliveryMethod.Unreliable);
            //SendArea(entity.Value.Position, message, workerId, NetDeliveryMethod.Unreliable);
        }

        private void OnEntityEvent(EventRequest eventRequest)
        {
            var entity = _entities.GetEntity(eventRequest.Header.EntityId);
            if (entity == null)
                return;

            var message = new MmoMessage()
            {
                MessageId = ServerCodes.EntityEvent,

                Info = _serializer.Serialize(eventRequest),
            };

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Sending Entity Event {eventRequest.Header.ComponentId}-{eventRequest.Header.EventId}");
            SendSubscribed(entity, message, new RemoteWorkerIdentifier(0), NetDeliveryMethod.ReliableUnordered);
            //SendArea(entity.Value.Position, message, 0, NetDeliveryMethod.ReliableUnordered);
        }

        private void OnEntityCommand(CommandRequest commandRequest)
        {

            var entity = _entities.GetEntity(commandRequest.Header.EntityId);
            if (entity == null)
                return;

            //todo: get ACL and find who has authority over the command
            var acls = entity.Acls;
            Acl? entityAcl = null;
            foreach (var acl in acls.AclList)
            {
                if (acl.ComponentId != commandRequest.Header.ComponentId)
                    continue;
                entityAcl = acl;
            }
            if (entityAcl == null)
                return;

            if (entityAcl.Value.WorkerType != WorkerType)
                return;

            var workerId = new RemoteWorkerIdentifier(entityAcl.Value.WorkerId);

            var message = new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandRequest,

                Info = _serializer.Serialize(commandRequest),
            };

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Sending Command Request {commandRequest.Header.RequestId} {commandRequest.Header.ComponentId}-{commandRequest.Header.CommandId}");

            if (workerId.IsValid())
            {
                if (_connections.TryGetValue(workerId, out var connection))
                {
                    SendToWorker(connection, message, NetDeliveryMethod.ReliableUnordered);
                }
                else
                {
                    _logger.LogError($"Could not find worker {workerId}");
                }
            }
            else
            {
                // Initial login so the authority is not set. Broadcast
                NetOutgoingMessage om = s_server.CreateMessage();
                om.Write(_serializer.Serialize(message));
                SendToAll(message, NetDeliveryMethod.ReliableUnordered);
            }


        }

        private void OnEntityCommandResponse(CommandResponse commandResponse)
        {

            var entity = _entities.GetEntity(commandResponse.Header.EntityId);
            if (entity == null)
                return;

            //todo: we need an internal request table
            var workerId = new RemoteWorkerIdentifier(commandResponse.Header.RequesterId);
            LidgrenWorkerConnection worker;
            if (!_connections.TryGetValue(workerId, out worker))
            {
                //disconnected??
                return;
            }

            //fix / validate data
            var headerToSend = commandResponse.Header;
            headerToSend.RequesterId = worker.WorkerId.Id;
            commandResponse.Header = headerToSend;

            var message = new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandResponse,

                Info = _serializer.Serialize(commandResponse),
            };
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Sending Command Response {commandResponse.Header.RequestId} {commandResponse.Header.ComponentId}-{commandResponse.Header.CommandId}");
            SendToWorker(worker, message, NetDeliveryMethod.ReliableUnordered);
        }

        private void ProcessOnEntityAddSubscription(EntityId entityId, IEnumerable<RemoteWorkerIdentifier> workers)
        {
            //send as create
            //probably should be buffered for 1 tick
            foreach (var workerId in workers)
            {
                if (!_connections.TryGetValue(workerId, out LidgrenWorkerConnection worker))
                    continue;
                HandleEntitySubChange(worker, true, entityId);

            }
        }

        private void ProcessOnEntityRemoveSubscription(EntityId entityId, IEnumerable<RemoteWorkerIdentifier> workers)
        {
            //send as remove
            //probably should be buffered for 1 tick
            foreach (var workerId in workers)
            {
                if (!_connections.TryGetValue(workerId, out LidgrenWorkerConnection worker))
                    continue;
                HandleEntitySubChange(worker, false, entityId);
            }
        }

        private void HandleEntitySubChange(LidgrenWorkerConnection worker, bool add, EntityId entityId)
        {
            if (add)
            {
                if (worker.EntitiesToRemove.ContainsKey(entityId))
                {
                    worker.EntitiesToRemove.TryRemove(entityId, out EntityId val);
                    return;
                }
                if (!worker.EntitiesToAdd.ContainsKey(entityId))
                    worker.EntitiesToAdd.TryAdd(entityId, entityId);
            }
            else
            {
                if (worker.EntitiesToAdd.ContainsKey(entityId))
                {
                    worker.EntitiesToAdd.TryRemove(entityId, out EntityId val);
                    return;
                }
                if (!worker.EntitiesToRemove.ContainsKey(entityId))
                    worker.EntitiesToRemove.TryAdd(entityId, entityId);
            }

            if (!_workerWithSubChanges.ContainsKey(worker.WorkerId))
                _workerWithSubChanges.TryAdd(worker.WorkerId, worker);
        }
    }
}
