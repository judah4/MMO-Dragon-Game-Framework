using Lidgren.Network;
using System.Runtime.Serialization;
using Mmogf.Core.Behaviors;
using Mmogf.Core.Contracts;
using Mmogf.Core.Contracts.Commands;
using Mmogf.Core.Contracts.Events;
using Mmogf.Core.Networking;
using Mmogf.Servers.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{

    public enum LogLevel
    {
        Debug,
        Warning,
        Error,
    }

    public class MmoWorker
    {

        public long ClientId => s_client.UniqueIdentifier;
        public string WorkerType { get; private set; }
        public int Ping { get; private set; }
        public DataStatistics ReceivedStats => _dataStatisticsReceived;
        public DataStatistics SentStats => _dataStatisticsSent;

        private NetClient s_client;
        private DateTime _pingRequestAt;
        private Dictionary<string, CommandHolder> _commandCallbacks = new Dictionary<string, CommandHolder>(1000);
        List<KeyValuePair<string, CommandHolder>> _commandTimeoutUpdates = new List<KeyValuePair<string, CommandHolder>>(100);
        CommandHolderCache _commandHolderCache;
        DataStatistics _dataStatisticsReceived;
        DataStatistics _dataStatisticsSent;
        //MessagePackSerializerOptions _compressOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);


        public bool Connected => s_client.ConnectionStatus == NetConnectionStatus.Connected;
        public NetConnectionStatus Status => s_client.ConnectionStatus;

        public event Action<EntityInfo> OnEntityCreation;
        public event Action<EntityCheckout> OnEntityCheckout;
        public event Action<CommandRequest> OnEntityCommand;
        public event Action<EntityUpdate> OnEntityUpdate;
        public event Action<EventRequest> OnEntityEvent;
        public event Action<EntityInfo> OnEntityDelete;
        public event Action OnConnect;

        public event Action<LogLevel, string> OnLog;

        List<IInternalBehavior> _internalBehaviors;

        public MmoWorker(NetPeerConfiguration config)
        {
            //config.AutoFlushSendQueue = false;
            s_client = new NetClient(config);
            _commandHolderCache = new CommandHolderCache();
            //s_client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage), _sync);
            _dataStatisticsReceived = new DataStatistics(this);
            _dataStatisticsSent = new DataStatistics(this);
            _internalBehaviors = new List<IInternalBehavior>()
            {
                new PingBehavior(this),
                _dataStatisticsReceived,
                _dataStatisticsSent,
            };

        }

        public void Connect(string workerType, string host, short port)
        {
            WorkerType = workerType;
            s_client.Start();
            NetOutgoingMessage hail = s_client.CreateMessage(workerType);
            s_client.Connect(host, port, hail);
        }

        public void Stop()
        {
            s_client.Disconnect("Requested by user");
        }

        public void Update()
        {
            if (!s_client.Configuration.AutoFlushSendQueue)
                s_client.FlushSendQueue();

            GotMessage(s_client);

            InternalBehaviors();
            CommandTimeouts();

            //Debug.Log($"{WorkerType} {s_client.Statistics.ToString()}");

        }

        private void InternalBehaviors()
        {
            for (int cnt = 0; cnt < _internalBehaviors.Count; cnt++)
            {
                _internalBehaviors[cnt].Update();
            }
        }

        private void CommandTimeouts()
        {
            if (_commandTimeoutUpdates.Count > 0)
                _commandTimeoutUpdates.Clear();

            foreach (var commandTimeout in _commandCallbacks)
            {
                var holder = commandTimeout.Value;
                holder.TimeoutTimer -= Time.deltaTime;

                if (holder.TimeoutTimer <= 0)
                {
                    _commandTimeoutUpdates.Add(commandTimeout);
                }
            }

            foreach (var update in _commandTimeoutUpdates)
            {
                var holder = update.Value;

                var response = new CommandResponse(CommandResponseHeader.Create(holder.Request.Header, CommandStatus.Timeout, "Request timed out with no response."), null);
                holder.SendResponse(response);
                _commandCallbacks.Remove(update.Key);
                _commandHolderCache.Release(holder);
            }

        }

        public void GotMessage(object peer)
        {
            NetIncomingMessage im;
            while ((im = s_client.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                        OnLog?.Invoke(LogLevel.Debug, im.ReadString());
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        OnLog?.Invoke(LogLevel.Error, im.ReadString());
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        OnLog?.Invoke(LogLevel.Warning, im.ReadString());
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        OnLog?.Invoke(LogLevel.Debug, text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                        {
                            OnLog?.Invoke(LogLevel.Debug, "Client connected");
                            //if self
                            OnConnect?.Invoke();
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {

                        }

                        //string reason = im.ReadString();
                        //OnLog(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        //OnLog?.Invoke("Client " + s_client.UniqueIdentifier + " Data: " + BitConverter.ToString(im.Data));
                        var simpleData = MessagePackSerializer.Deserialize<MmoMessage>(im.Data);
                        switch ((ServerCodes)simpleData.MessageId)
                        {
                            case ServerCodes.ClientConnect:
                                //ClientId = ClientConnect.Parser.ParseFrom(simpleData.Info).ClientId;w
                                //OnLog?.Invoke("My Client Id is " + ClientId);
                                //OnLog?.Invoke("Client connected msg");
                                //OnConnect?.Invoke();
                                break;
                            case ServerCodes.EntityInfo:
                                var entityInfo = MessagePackSerializer.Deserialize<EntityInfo>(simpleData.Info);
                                //OnLog?.Invoke($"Client Entity Info: {entityInfo.EntityId}");
                                //foreach (var pair in entityInfo.EntityData)
                                //{
                                //    OnLog?.Invoke($"{pair.Key} {BitConverter.ToString(pair.Value)}");
                                //}

                                OnEntityCreation?.Invoke(entityInfo);
                                _dataStatisticsReceived.RecordMessage(entityInfo.EntityId.Id, im.LengthBytes, DataStat.Entity);
                                break;
                            case ServerCodes.EntityCheckout:
                                var entityCheckout = MessagePackSerializer.Deserialize<EntityCheckout>(simpleData.Info);

                                OnEntityCheckout?.Invoke(entityCheckout);
                                break;
                            case ServerCodes.EntityUpdate:
                                var entityUpdate = MessagePackSerializer.Deserialize<EntityUpdate>(simpleData.Info);
                                OnEntityUpdate?.Invoke(entityUpdate);
                                _dataStatisticsReceived.RecordMessage(entityUpdate.ComponentId, im.LengthBytes, DataStat.Update);
                                break;
                            case ServerCodes.EntityDelete:
                                var entityDelete = MessagePackSerializer.Deserialize<EntityInfo>(simpleData.Info);
                                OnEntityDelete?.Invoke(entityDelete);
                                _dataStatisticsReceived.RecordMessage(entityDelete.EntityId.Id, im.LengthBytes, DataStat.Entity);
                                break;
                            case ServerCodes.EntityEvent:
                                var eventRequest = MessagePackSerializer.Deserialize<EventRequest>(simpleData.Info);
                                OnEntityEvent?.Invoke(eventRequest);
                                _dataStatisticsReceived.RecordMessage(eventRequest.Header.EventId, im.LengthBytes, DataStat.Event);
                                break;
                            case ServerCodes.EntityCommandRequest:
                                var commandRequest = MessagePackSerializer.Deserialize<CommandRequest>(simpleData.Info);
                                OnEntityCommand?.Invoke(commandRequest);
                                _dataStatisticsReceived.RecordMessage(commandRequest.Header.CommandId, im.LengthBytes, DataStat.Command);
                                break;
                            case ServerCodes.EntityCommandResponse:
                                var commandResponse = MessagePackSerializer.Deserialize<CommandResponse>(simpleData.Info);

                                CommandHolder callback;
                                //get from dictionary
                                if (_commandCallbacks.TryGetValue(commandResponse.Header.RequestId, out callback))
                                {
                                    //send to callback
                                    _commandCallbacks.Remove(commandResponse.Header.RequestId);
                                    try
                                    {
                                        callback.SendResponse(commandResponse);
                                    }
                                    catch (Exception e)
                                    {
                                        OnLog?.Invoke(LogLevel.Error, e.ToString());
                                    }
                                    _commandHolderCache.Release(callback);
                                }
                                _dataStatisticsReceived.RecordMessage(commandResponse.Header.CommandId, im.LengthBytes, DataStat.Command);
                                break;
                            case ServerCodes.Ping:
                                PingResponse();
                                //record ping?
                                _dataStatisticsReceived.RecordMessage(World.PingCommand.CommandId, im.LengthBytes, DataStat.Command);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        OnLog?.Invoke(LogLevel.Error, "Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                s_client.Recycle(im);
            }
        }

        public int Send(MmoMessage message, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableUnordered, int sequence = 0)
        {
            var bytes = MessagePackSerializer.Serialize(message);
            NetOutgoingMessage om = s_client.CreateMessage(bytes.Length);
            om.Write(bytes);
            s_client.SendMessage(om, deliveryMethod, sequence);
            return bytes.Length;
        }

        public void SendInterestChange(Position position)
        {
            var changeInterest = new ChangeInterestArea()
            {
                Position = position.ToFixedVector3(),
            };

            var byteLength = Send(new MmoMessage()
            {
                MessageId = ServerCodes.ChangeInterestArea,
                Info = MessagePackSerializer.Serialize(changeInterest),
            });
            _dataStatisticsSent.RecordMessage(World.ChangeInterestAreaCommand.CommandId, byteLength, DataStat.Command);
        }

        public void SendEntityUpdate<T>(EntityId entityId, short componentId, T message)
        {

            var entityUpdate = new EntityUpdate()
            {
                EntityId = entityId,
                ComponentId = componentId,
                Info = MessagePackSerializer.Serialize(message),
            };

            var deliveryMethod = NetDeliveryMethod.ReliableUnordered;
            var sequence = 0;

            if (componentId == FixedVector3.ComponentId)
            {
                //special
                deliveryMethod = NetDeliveryMethod.UnreliableSequenced;
                sequence = 1;
            }
            if (componentId == Rotation.ComponentId)
            {
                //special
                deliveryMethod = NetDeliveryMethod.UnreliableSequenced;
                sequence = 2;
            }

            var byteLength = Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityUpdate,
                Info = MessagePackSerializer.Serialize(entityUpdate),
            }, deliveryMethod, sequence);
            _dataStatisticsSent.RecordMessage(componentId, byteLength, DataStat.Update);
        }

        public void SendPing()
        {
            var byteLength = Send(new MmoMessage()
            {
                MessageId = ServerCodes.Ping,
                Info = new byte[0],
            });
            _pingRequestAt = DateTime.UtcNow;
            _dataStatisticsSent.RecordMessage(World.PingCommand.CommandId, byteLength, DataStat.Command);
        }

        void PingResponse()
        {
            var timespan = DateTime.UtcNow - _pingRequestAt;
            Ping = (int)timespan.TotalMilliseconds;
        }
        public string SendCommand<T, TRequest, TResponse>(EntityId entityId, short componentId, T command, Action<CommandResult<T, TRequest, TResponse>> callback, float timeout = 10f) where T : ICommandBase<TRequest, TResponse> where TRequest : struct where TResponse : struct
        {
            var requestId = Guid.NewGuid().ToString();

            command.Response = null; //clear data
            var request = new CommandRequest()
            {
                Header = new CommandRequestHeader()
                {
                    RequestId = requestId,
                    RequestorWorkerType = WorkerType,
                    EntityId = entityId,
                    ComponentId = componentId,
                    CommandId = command.GetCommandId(),
                },
                Payload = MessagePackSerializer.Serialize(command),
            };

            //register callback
            var holder = _commandHolderCache.Get(request, command, callback, timeout);
            _commandCallbacks.Add(requestId, holder);

            var byteLength = Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandRequest,
                Info = MessagePackSerializer.Serialize(request),
            });
            _dataStatisticsSent.RecordMessage(command.GetCommandId(), byteLength, DataStat.Command);
            return requestId;
        }

        public void SendCommandResponse<T, TRequest, TResponse>(CommandRequest request, T command) where T : ICommandBase<TRequest, TResponse> where TRequest : struct where TResponse : struct
        {
            command.Request = null; //clear data

            var byteLength = Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandResponse,
                Info = MessagePackSerializer.Serialize(new CommandResponse(CommandResponseHeader.Create(request.Header, CommandStatus.Success, ""), MessagePackSerializer.Serialize(command))),
            });
            _dataStatisticsSent.RecordMessage(request.Header.CommandId, byteLength, DataStat.Command);
        }

        public void SendCommandResponseFailure(CommandRequest request, CommandStatus status, string message)
        {
            if (message == null)
                message = "Something went wrong.";

            var byteLength = Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandResponse,
                Info = MessagePackSerializer.Serialize(new CommandResponse(CommandResponseHeader.Create(request.Header, status, message), null)),
            });
            _dataStatisticsSent.RecordMessage(request.Header.CommandId, byteLength, DataStat.Command);
        }

        public void SendEvent<T>(EntityId entityId, short componentId, T eventPayload) where T : IEvent
        {
            var byteLength = Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityEvent,
                Info = MessagePackSerializer.Serialize(new EventRequest()
                {
                    Header = new EventRequestHeader(
                        entityId,
                        componentId,
                        eventPayload.GetEventId()),
                    Payload = MessagePackSerializer.Serialize(eventPayload),
                }),
            });
            _dataStatisticsSent.RecordMessage(eventPayload.GetEventId(), byteLength, DataStat.Event);
        }
    }
}