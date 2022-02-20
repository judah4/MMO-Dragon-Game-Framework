using System;
using System.Collections.Generic;
using System.Threading;
using Lidgren.Network;
using MessagePack;
using Mmogf.Core;

namespace MmoGameFramework
{
    public class MmoServer
    {
        private NetServer s_server;
        private EntityStore _entities;
        NetPeerConfiguration _config;
        public bool Active => s_server.Status == NetPeerStatus.Running;

        public Dictionary<long, WorkerConnection> _connections = new Dictionary<long, WorkerConnection>();

        public MmoServer(EntityStore entities, NetPeerConfiguration config)
        {
            _entities = entities;

            // set up network
            _config = config;
            s_server = new NetServer(_config);

            _entities.OnUpdateEntity += OnEntityUpdate;
            _entities.OnUpdateEntityPartial += OnEntityUpdatePartial;
        }

        public void Start()
        {
            s_server.Start();

            var thread1 = new Thread(Loop);
            thread1.Start();

        }

        public void Stop()
        {
            //send stop to peers
            s_server.Shutdown("End");
        }

        void Loop()
        {
            while (s_server.Status != NetPeerStatus.NotRunning)
            {
                try
                {

                    NetIncomingMessage im;
                    while ((im = s_server.ReadMessage()) != null)
                    {
                        // handle incoming message
                        switch (im.MessageType)
                        {
                            case NetIncomingMessageType.DebugMessage:
                            case NetIncomingMessageType.ErrorMessage:
                            case NetIncomingMessageType.WarningMessage:
                            case NetIncomingMessageType.VerboseDebugMessage:
                                string text = im.ReadString();
                                Console.WriteLine(text);
                                break;

                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                                string reason = im.ReadString();
                                Console.WriteLine(im.SenderConnection.RemoteUniqueIdentifier + " " + status + ": " + reason);

                                if (status == NetConnectionStatus.Connected)
                                {
                                    _connections.Add(im.SenderConnection.RemoteUniqueIdentifier, new WorkerConnection("Worker", im.SenderConnection, new Position()));
                                    Console.WriteLine("Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString());
                                    var message = new SimpleMessage()
                                    {
                                        MessageId = (int)ServerCodes.ClientConnect,
                                        Info = MessagePackSerializer.Serialize(new ClientConnect()
                                        {
                                            ClientId = im.SenderConnection.RemoteUniqueIdentifier,
                                        }),
                                    };
                                    NetOutgoingMessage om = s_server.CreateMessage();
                                    om.Write(MessagePackSerializer.Serialize(message));
                                    s_server.SendMessage(om, im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                                else if(status == NetConnectionStatus.Disconnected)
                                {
                                    Console.WriteLine("Server " + im.SenderConnection.RemoteUniqueIdentifier + " Disconnected");
                                    _connections.Remove(im.SenderConnection.RemoteUniqueIdentifier);
                                }

                                break;
                            case NetIncomingMessageType.Data:

                                Console.WriteLine(im.SenderConnection.RemoteUniqueIdentifier +" What is this data? '" + BitConverter.ToString(im.Data) + "'");
                                var simpleData = MessagePackSerializer.Deserialize<SimpleMessage>(im.Data);

                                switch ((ServerCodes)simpleData.MessageId)
                                {
                                    case ServerCodes.GameData:
                                        var gameData = MessagePackSerializer.Deserialize<GameData>(simpleData.Info);
                                        Console.WriteLine(
                                            $"Server Game Data: {BitConverter.ToString(gameData.Info)}");
                                        break;
                                    case ServerCodes.ChangeInterestArea:
                                        var interestArea = MessagePackSerializer.Deserialize<ChangeInterestArea>(simpleData.Info);
                                        WorkerConnection worker;
                                        if (_connections.TryGetValue(im.SenderConnection.RemoteUniqueIdentifier, out worker))
                                        {
                                            worker.InterestPosition = interestArea.Position;

                                            var entities = _entities.GetInArea(worker.InterestPosition,
                                                worker.InterestRange);

                                            foreach (var entityInfo in entities)
                                            {
                                                Send(im.SenderConnection, new SimpleMessage()
                                                {
                                                    MessageId = (int)ServerCodes.EntityInfo,
                                                    Info = MessagePackSerializer.Serialize(entityInfo),
                                                });
                                            }

                                        }

                                        break;
                                    case ServerCodes.EntityUpdate:
                                        HandleEntityUpdate(im, simpleData);
                                        break;
                                    default:
                                        // incoming chat message from a client
                                        //string chat = im.ReadString();

                                        //Console.WriteLine("What is this data? '" + chat + "'");
                                        break;
                                }
                                
                                break;
                            default:
                                Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                                break;
                        }
                        s_server.Recycle(im);
                    }

                    Thread.Sleep(1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        void HandleEntityUpdate(NetIncomingMessage im, SimpleMessage simpleData)
        {
            var entityUpdate = MessagePackSerializer.Deserialize<EntityUpdate>(simpleData.Info);
            var entityInfo = _entities.GetEntity(entityUpdate.EntityId);

            if (entityInfo == null)
                return;
            

            entityInfo.Value.EntityData.Remove(entityUpdate.ComponentId);
            entityInfo.Value.EntityData.Add(entityUpdate.ComponentId, entityUpdate.Info);


            if (entityUpdate.ComponentId == 2)
            {
                var position = MessagePackSerializer.Deserialize<Position>(entityUpdate.Info);
                Console.WriteLine($"Entiy: {entityInfo.Value.EntityId} position to {position.ToString()}");
            }


            _entities.UpdateEntityPartial(entityUpdate);

        }

        public void Send(NetConnection connection, object message)
        {
            NetOutgoingMessage om = s_server.CreateMessage();
            om.Write(MessagePackSerializer.Serialize(message));
            s_server.SendMessage(om, connection, NetDeliveryMethod.UnreliableSequenced);
        }

        public void SendArea(Position position, SimpleMessage message)
        {
            var connections = new List<NetConnection>();
            foreach (var workerConnection in _connections)
            {
                if (Position.WithinArea(position, workerConnection.Value.InterestPosition,
                    workerConnection.Value.InterestRange))
                {
                    connections.Add(workerConnection.Value.Connection);

                }
            }

            if(connections.Count < 1)
                return;

            NetOutgoingMessage om = s_server.CreateMessage();
            om.Write(MessagePackSerializer.Serialize(message));
            s_server.SendMessage(om, connections, NetDeliveryMethod.UnreliableSequenced, 0);
        }

        private void OnEntityUpdate(EntityInfo entityInfo)
        {
            var message = new SimpleMessage()
            {
                MessageId = (int)ServerCodes.EntityInfo,

                Info = MessagePackSerializer.Serialize(entityInfo),
            };

            Console.WriteLine("Sending Entity Info" );
            SendArea(entityInfo.Position, message);
        }

        private void OnEntityUpdatePartial(EntityUpdate entityUpdate)
        {

            var entity = _entities.GetEntity(entityUpdate.EntityId);
            if(entity == null)
                return;

            var message = new SimpleMessage()
            {
                MessageId = (int)ServerCodes.EntityUpdate,

                Info = MessagePackSerializer.Serialize(entityUpdate),
            };

            Console.WriteLine("Sending Entity Update");
            SendArea(entity.Value.Position, message);
        }

    }
}
