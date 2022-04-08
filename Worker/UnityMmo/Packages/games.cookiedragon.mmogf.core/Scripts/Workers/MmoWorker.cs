using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Lidgren.Network;
using MessagePack;
using Mmogf.Core.Behaviors;
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
        public class CommandHolder
        {
            public CommandRequest Request { get; set; }
            public Action<CommandResponse> Response { get; set; }
            public float TimeoutTimer { get; set; }

            public CommandHolder(CommandRequest request, Action<CommandResponse> response, float timeoutTimer)
            {
                Request = request;
                Response = response;
                TimeoutTimer = timeoutTimer;
            }

        }

        public long ClientId => s_client.UniqueIdentifier;
        public string WorkerType { get; private set; }
        public int Ping { get; private set; }

        private NetClient s_client;
        private DateTime _pingRequestAt;
        private Dictionary<string, CommandHolder> _commandCallbacks = new Dictionary<string, CommandHolder>();

        public bool Connected => s_client.ConnectionStatus == NetConnectionStatus.Connected;
        public NetConnectionStatus Status => s_client.ConnectionStatus;

        public event Action<EntityInfo> OnEntityCreation;
        public event Action<CommandRequest> OnEntityCommand;
        public event Action<EntityUpdate> OnEntityUpdate;
        public event Action<EventRequest> OnEntityEvent;
        public event Action<EntityInfo> OnEntityDelete;
        public event Action OnConnect;

        public event Action<LogLevel, string> OnLog;

        List<IInternalBehavior> _internalBehaviors;

        public MmoWorker(NetPeerConfiguration config)
        {
            config.AutoFlushSendQueue = false;
            s_client = new NetClient(config);
            //s_client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage), _sync);

            _internalBehaviors = new List<IInternalBehavior>()
            {
                new PingBehavior(this),
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
            s_client.FlushSendQueue();

            GotMessage(s_client);

            InternalBehaviors();
            CommandTimeouts();
        }

        private void InternalBehaviors()
        {
            for(int cnt = 0; cnt < _internalBehaviors.Count; cnt++)
            {
                _internalBehaviors[cnt].Update();
            }
        }

        private void CommandTimeouts()
        {
            Dictionary<string, CommandHolder> updates = new Dictionary<string, CommandHolder>(_commandCallbacks);

            foreach (var commandTimeout in updates)
            {
                var holder = commandTimeout.Value;
                holder.TimeoutTimer -= Time.deltaTime;

                if(holder.TimeoutTimer <= 0)
                {
                    var response = CommandResponse.Create(holder.Request, CommandStatus.Timeout, "Request timed out with no response.");
                    holder.Response?.Invoke(response);
                    _commandCallbacks.Remove(commandTimeout.Key);
                }
            }

            //foreach(var update in updates)
            //{
            //    _commandCallbacks[update.Key] = update.Value;
            //}

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
                                break;
                            case ServerCodes.EntityUpdate:
                                var entityUpdate = MessagePackSerializer.Deserialize<EntityUpdate>(simpleData.Info);
                                OnEntityUpdate?.Invoke(entityUpdate);
                                break;
                            case ServerCodes.EntityDelete:
                                var entityDelete = MessagePackSerializer.Deserialize<EntityInfo>(simpleData.Info);
                                OnEntityDelete?.Invoke(entityDelete);
                                break;
                            case ServerCodes.EntityEvent:
                                var eventRequest = MessagePackSerializer.Deserialize<EventRequest>(simpleData.Info);
                                OnEntityEvent?.Invoke(eventRequest);
                                break;
                            case ServerCodes.EntityCommandRequest:
                            var commandRequest = MessagePackSerializer.Deserialize<CommandRequest>(simpleData.Info);
                            OnEntityCommand?.Invoke(commandRequest);
                            break;
                            case ServerCodes.EntityCommandResponse:
                                var commandResponse = MessagePackSerializer.Deserialize<CommandResponse>(simpleData.Info);

                                CommandHolder callback;
                                //get from dictionary
                                if (_commandCallbacks.TryGetValue(commandResponse.RequestId, out callback))
                                {
                                    //send to callback
                                    _commandCallbacks.Remove(commandResponse.RequestId);
                                    try
                                    {
                                        callback.Response?.Invoke(commandResponse);
                                    }
                                    catch(Exception e)
                                    {
                                        OnLog?.Invoke(LogLevel.Error, e.ToString());
                                    }
                                }
                                break;
                            case ServerCodes.Ping:
                            PingResponse();
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

        public void Send(MmoMessage message)
        {
            NetOutgoingMessage om = s_client.CreateMessage();
            om.Write(MessagePackSerializer.Serialize(message));
            s_client.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
        }

        public void SendInterestChange(Position position)
        {
            var changeInterest = new ChangeInterestArea()
            {
                Position = position,
            };

            Send(new MmoMessage()
            {
                MessageId = ServerCodes.ChangeInterestArea,
                Info = MessagePackSerializer.Serialize(changeInterest),
            });
        }

        public void SendEntityUpdate<T>(int entityId, int componentId, T message) where T : IMessage
        {

            var changeInterest = new EntityUpdate()
            {
                EntityId = entityId,
                ComponentId = componentId,
                Info = MessagePackSerializer.Serialize(message),
            };

            Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityUpdate,
                Info = MessagePackSerializer.Serialize(changeInterest),
            });
        }

        public void SendPing()
        {
            Send(new MmoMessage()
            {
                MessageId = ServerCodes.Ping,
                Info = new byte[0],
            });
            _pingRequestAt = DateTime.UtcNow;
        }

        void PingResponse()
        {
            var timespan = DateTime.UtcNow - _pingRequestAt;
            Ping = (int)timespan.TotalMilliseconds;
            OnLog?.Invoke(LogLevel.Debug, $"Ping: {Ping} - {WorkerType}");
        }
        public void SendCommand<T>(int entityId, int componentId, T command, Action<CommandResponse> callback) where T : ICommand
        {
            var requestId = Guid.NewGuid().ToString();

            var request = new CommandRequest()
            {
                RequestId = requestId,
                RequestorWorkerType = WorkerType,
                EntityId = entityId,
                ComponentId = componentId,
                CommandId = command.GetCommandId(),
                Payload = MessagePackSerializer.Serialize(command),
            };

            //register callback
            _commandCallbacks.Add(requestId, new CommandHolder(request, callback, 10f));

            Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandRequest,
                Info = MessagePackSerializer.Serialize(request),
            });
        }

        public void SendCommandResponse<T>(CommandRequest request, T responsePayload) where T : ICommand
        {
            Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandResponse,
                Info = MessagePackSerializer.Serialize(CommandResponse.Create(request, CommandStatus.Success, "", MessagePackSerializer.Serialize(responsePayload))),
            });
        }

        public void SendEvent<T>(int entityId, int componentId, T eventPayload) where T : IEvent
        {
            Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityEvent,
                Info = MessagePackSerializer.Serialize(new EventRequest()
                {
                    EntityId = entityId,
                    ComponentId = componentId,
                    EventId = eventPayload.GetEventId(),
                    Payload = MessagePackSerializer.Serialize(eventPayload),
                }),
            });
        }


    }

}