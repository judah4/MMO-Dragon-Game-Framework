using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Lidgren.Network;
using MessagePack;
using Mmogf.Core.Behaviors;

namespace Mmogf.Core {

    public class MmoWorker
    {
        public long ClientId { get; private set; }
        public string WorkerType { get; private set; }
        public int Ping { get; private set; }

        private NetClient s_client;
        private DateTime _pingRequestAt;
        private Dictionary<string, Action<CommandResponse>> _commandCallbacks = new Dictionary<string, Action<CommandResponse>>();

        public bool Connected => s_client.ConnectionStatus == NetConnectionStatus.Connected;
        public NetConnectionStatus Status => s_client.ConnectionStatus;

        public event Action<EntityInfo> OnEntityCreation;
        public event Action<CommandRequest> OnEntityCommand;
        public event Action<EntityUpdate> OnEntityUpdate;
        public event Action OnConnect;

        public event Action<string> OnLog;

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
            GotMessage(s_client);

            InternalBehaviors();
        }

        private void InternalBehaviors()
        {
            for(int cnt = 0; cnt < _internalBehaviors.Count; cnt++)
            {
                _internalBehaviors[cnt].Update();
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
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        OnLog?.Invoke(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                        {
                            OnLog?.Invoke("Client connected");
                            //if self
                            OnConnect?.Invoke();
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {

                        }

                        string reason = im.ReadString();
                        OnLog(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        OnLog?.Invoke("Client " + s_client.UniqueIdentifier + " Data: " + BitConverter.ToString(im.Data));
                        var simpleData = MessagePackSerializer.Deserialize<SimpleMessage>(im.Data);
                        switch ((ServerCodes)simpleData.MessageId)
                        {
                            case ServerCodes.ClientConnect:
                                //ClientId = ClientConnect.Parser.ParseFrom(simpleData.Info).ClientId;w
                                //OnLog?.Invoke("My Client Id is " + ClientId);
                                OnLog?.Invoke("Client connected msg");
                                //OnConnect?.Invoke();
                                break;
                            case ServerCodes.GameData:
                                var gameData = MessagePackSerializer.Deserialize<GameData>(simpleData.Info);
                                OnLog?.Invoke($"Client Game Data: {BitConverter.ToString(gameData.Info)}");

                                break;
                            case ServerCodes.EntityInfo:
                                var entityInfo = MessagePackSerializer.Deserialize<EntityInfo>(simpleData.Info);
                                OnLog?.Invoke($"Client Entity Info: {entityInfo.EntityId}");
                                foreach (var pair in entityInfo.EntityData)
                                {
                                    OnLog?.Invoke($"{pair.Key} {BitConverter.ToString(pair.Value)}");
                                }

                                OnEntityCreation?.Invoke(entityInfo);
                                break;
                            case ServerCodes.EntityUpdate:
                                var entityUpdate = MessagePackSerializer.Deserialize<EntityUpdate>(simpleData.Info);
                                OnEntityUpdate?.Invoke(entityUpdate);
                                break;
                        case ServerCodes.EntityCommandRequest:
                            var commandRequest = MessagePackSerializer.Deserialize<CommandRequest>(simpleData.Info);
                            OnEntityCommand?.Invoke(commandRequest);
                            break;
                            case ServerCodes.EntityCommandResponse:
                                var commandResponse = MessagePackSerializer.Deserialize<CommandResponse>(simpleData.Info);

                                Action<CommandResponse> callback;
                                //get from dictionary
                                if (_commandCallbacks.TryGetValue(commandResponse.RequestId, out callback))
                                {
                                    //send to callback
                                    _commandCallbacks.Remove(commandResponse.RequestId);
                                    try
                                    {
                                        callback?.Invoke(commandResponse);
                                    }
                                    catch(Exception e)
                                    {
                                        OnLog?.Invoke(e.ToString());
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
                        OnLog?.Invoke("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                s_client.Recycle(im);
            }
        }

        public void Send(SimpleMessage message)
        {
            NetOutgoingMessage om = s_client.CreateMessage();
            om.Write(MessagePackSerializer.Serialize(message));
            s_client.SendMessage(om, NetDeliveryMethod.UnreliableSequenced);
            s_client.FlushSendQueue();
        }

        public void SendInterestChange(Position position)
        {
            var changeInterest = new ChangeInterestArea()
            {
                Position = position,
            };

            Send(new SimpleMessage()
            {
                MessageId = (int)ServerCodes.ChangeInterestArea,
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

            Send(new SimpleMessage()
            {
                MessageId = (int)ServerCodes.EntityUpdate,
                Info = MessagePackSerializer.Serialize(changeInterest),
            });
        }

        public void SendPing()
        {
            Send(new SimpleMessage()
            {
                MessageId = (int)ServerCodes.Ping,
                Info = new byte[0],
            });
            _pingRequestAt = DateTime.UtcNow;
        }

        void PingResponse()
        {
            var timespan = DateTime.UtcNow - _pingRequestAt;
            Ping = (int)timespan.TotalMilliseconds;
            OnLog?.Invoke($"Ping: {Ping}");
        }
        public void SendCommand<T>(int entityId, int componentId, T command, Action<CommandResponse> callback) where T : ICommand
        {

            var requestId = Guid.NewGuid().ToString();

            //register callback
            _commandCallbacks.Add(requestId, callback);

            Send(new SimpleMessage()
            {
                MessageId = (int)ServerCodes.EntityCommandRequest,
                Info = MessagePackSerializer.Serialize(new CommandRequest()
                {
                    RequestId = requestId,
                    RequestorWorkerType = WorkerType,
                    EntityId = entityId,
                    ComponentId = componentId,
                    Payload = MessagePackSerializer.Serialize(command),
                }),
            });
        }

        internal void SendCommandResponse<T>(CommandRequest request, T responsePayload) where T : ICommand
        {
            Send(new SimpleMessage()
            {
                MessageId = (int)ServerCodes.EntityCommandResponse,
                Info = MessagePackSerializer.Serialize(new CommandResponse()
                {
                    RequestId = request.RequestId,
                    CommandStatus = CommandStatus.Success,
                    RequesterId = request.RequesterId,
                    EntityId = request.EntityId,
                    ComponentId = request.ComponentId,
                    Payload = MessagePackSerializer.Serialize(responsePayload),
                }),
            });
        }


    }

}