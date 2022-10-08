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

            public float TimeoutTimer { get; set; }

            public CommandHolder(CommandRequest request, float timeoutTimer)
            {
                Request = request;
                TimeoutTimer = timeoutTimer;
            }

            public virtual void SendResponse(CommandResponse response)
            {

            }

        }

        public class CommandHolderTyped<TCommand, TRequest, TResponse> : CommandHolder where TCommand : ICommandBase<TRequest, TResponse> where TRequest : struct where TResponse : struct
        {
            public ICommandBase<TRequest,TResponse> Command { get; set; }
            public Action<CommandResult<TCommand,TRequest, TResponse>> Response { get; set; }

            public CommandHolderTyped(CommandRequest request, ICommandBase<TRequest, TResponse> command, Action<CommandResult<TCommand, TRequest, TResponse>> response, float timeoutTimer) : base(request, timeoutTimer)
            {
                Response = response;
                Command = command;
            }

            public override void SendResponse(CommandResponse response)
            {

                TRequest? requestPayload = null;
                TResponse? responsePayload = null;
                if(response.Payload != null)
                {
                    var command = MessagePackSerializer.Deserialize<TCommand>(response.Payload);
                    requestPayload = command.Request;
                    responsePayload = command.Response;
                }

                var result = CommandResult<TCommand,TRequest,TResponse>.Create(response, requestPayload, responsePayload);
               Response?.Invoke(result);
            }

        }

        public long ClientId => s_client.UniqueIdentifier;
        public string WorkerType { get; private set; }
        public int Ping { get; private set; }

        private NetClient s_client;
        private DateTime _pingRequestAt;
        private Dictionary<string, CommandHolder> _commandCallbacks = new Dictionary<string, CommandHolder>(1000);
        List<KeyValuePair<string, CommandHolder>> _commandTimeoutUpdates = new List<KeyValuePair<string, CommandHolder>>(100);


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
            if (_commandTimeoutUpdates.Count > 0)
                _commandTimeoutUpdates.Clear();

            foreach (var commandTimeout in _commandCallbacks)
            {
                var holder = commandTimeout.Value;
                holder.TimeoutTimer -= Time.deltaTime;

                if(holder.TimeoutTimer <= 0)
                {
                    _commandTimeoutUpdates.Add(commandTimeout);
                }
            }

            foreach (var update in _commandTimeoutUpdates)
            {
                var holder = update.Value;

                var response = CommandResponse.Create(holder.Request, CommandStatus.Timeout, "Request timed out with no response.");
                holder.SendResponse(response);
                _commandCallbacks.Remove(update.Key);
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
                                break;
                            case ServerCodes.EntityCheckout:
                                var entityCheckout = MessagePackSerializer.Deserialize<EntityCheckout>(simpleData.Info);

                                OnEntityCheckout?.Invoke(entityCheckout);
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
                                        callback.SendResponse(commandResponse);
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
            //OnLog?.Invoke(LogLevel.Debug, $"Ping: {Ping} - {WorkerType}");
        }
        public void SendCommand<T,TRequest,TResponse>(int entityId, int componentId, T command, Action<CommandResult<T, TRequest, TResponse>> callback) where T : ICommandBase<TRequest,TResponse> where TRequest : struct where TResponse : struct
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
            _commandCallbacks.Add(requestId, new CommandHolderTyped<T, TRequest, TResponse>(request, command, callback, 10f));

            Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandRequest,
                Info = MessagePackSerializer.Serialize(request),
            });
        }

        public void SendCommandResponse<T, TRequest, TResponse>(CommandRequest request, T command) where T : ICommandBase<TRequest, TResponse> where TRequest : struct where TResponse : struct
        {
            Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandResponse,
                Info = MessagePackSerializer.Serialize(CommandResponse.Create(request, CommandStatus.Success, "", MessagePackSerializer.Serialize(command))),
            });
        }

        public void SendCommandResponseFailure(CommandRequest request, CommandStatus status, string message)
        {
            if(message == null)
                message = "Something went wrong.";

            Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityCommandResponse,
                Info = MessagePackSerializer.Serialize(CommandResponse.Create(request, status, message, null)),
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