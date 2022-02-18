using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;
using Lidgren.Network;
using MessageProtocols;
using MessageProtocols.Core;
using MessageProtocols.Server;

namespace MmoGameFramework
{
    public class MmoServer
    {
        private NetServer s_server;
        private EntityStore _entities;
        NetPeerConfiguration _config;
        public bool Active => s_server.Status == NetPeerStatus.Running;

        public Dictionary<long, WorkerConnection> _connections = new Dictionary<long, WorkerConnection>();

        public MmoServer(EntityStore entities)
        {
            _entities = entities;

            // set up network
            _config = new NetPeerConfiguration("dragon-bingus");
            _config.MaximumConnections = 100;
            _config.Port = 14242;
            s_server = new NetServer(_config);

            _entities.OnUpdateEntity += OnEntityUpdate;
            _entities.OnUpdateEntityPartial += OnEntityUpdatePartial;
        }

        public void Start(short port)
        {
            _config.Port = port;
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
                                Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                                if (status == NetConnectionStatus.Connected)
                                {
                                    _connections.Add(im.SenderConnection.RemoteUniqueIdentifier, new WorkerConnection("Worker", im.SenderConnection, new Position()));
                                    Console.WriteLine("Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString());
                                    var message = new SimpleMessage()
                                    {
                                        MessageId = (int)ServerCodes.ClientConnect,
                                        Info = new ClientConnect()
                                        {
                                            ClientId = (int)im.SenderConnection.RemoteUniqueIdentifier,
                                        }.ToByteString(),
                                    };
                                    NetOutgoingMessage om = s_server.CreateMessage();
                                    om.Write(message.ToByteArray());
                                    s_server.SendMessage(om, im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                                else if(status == NetConnectionStatus.Disconnected)
                                {
                                    Console.WriteLine("Server " + (int)im.SenderConnection.RemoteUniqueIdentifier + " Disconnected");
                                    _connections.Remove((int)im.SenderConnection.RemoteUniqueIdentifier);
                                }

                                break;
                            case NetIncomingMessageType.Data:

                                var simpleData = SimpleMessage.Parser.ParseFrom(im.Data);

                                Console.WriteLine("Server " + im.SenderConnection.RemoteUniqueIdentifier);

                                switch ((ServerCodes)simpleData.MessageId)
                                {
                                    case ServerCodes.GameData:
                                        var gameData = GameData.Parser.ParseFrom(simpleData.Info);
                                        Console.WriteLine(
                                            $"Server Game Data: {BitConverter.ToString(gameData.Info.ToByteArray())}");
                                        break;
                                    case ServerCodes.ChangeInterestArea:
                                        var interestArea = ChangeInterestArea.Parser.ParseFrom(simpleData.Info);
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
                                                    Info = new EntityInfo(entityInfo).ToByteString(),
                                                });
                                            }

                                        }

                                        break;
                                    case ServerCodes.EntityUpdate:
                                        HandleEntityUpdate(im, simpleData);
                                        break;
                                    default:
                                        // incoming chat message from a client
                                        string chat = im.ReadString();

                                        Console.WriteLine("Broadcasting '" + chat + "'");

                                        // broadcast this to all connections, except sender
                                        List<NetConnection> all = s_server.Connections; // get copy
                                        all.Remove(im.SenderConnection);

                                        if (all.Count > 0)
                                        {
                                            NetOutgoingMessage om = s_server.CreateMessage();
                                            om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
                                            s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                                        }
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
            var entityUpdate = EntityUpdate.Parser.ParseFrom(simpleData.Info);
            var entityInfo = _entities.GetEntity(entityUpdate.EntityId);

            if (entityInfo == null)
            {
                return;

            }

            entityInfo.EntityData.Remove(entityUpdate.ComponentId);
            entityInfo.EntityData.Add(entityUpdate.ComponentId, entityUpdate.Info);


            if (entityUpdate.ComponentId == 2)
            {
                var position = Position.Parser.ParseFrom(entityUpdate.Info);
                Console.WriteLine($"Entiy: {entityInfo.EntityId} position to {position.ToString()}");
            }


            _entities.UpdateEntityPartial(entityUpdate);

        }

        public void Send(NetConnection connection, IMessage message)
        {
            NetOutgoingMessage om = s_server.CreateMessage();
            om.Write(message.ToByteArray());
            s_server.SendMessage(om, connection, NetDeliveryMethod.Unreliable, 0);
        }

        public void SendArea(Position position, IMessage message)
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
            om.Write(message.ToByteArray());
            s_server.SendMessage(om, connections, NetDeliveryMethod.Unreliable, 0);
        }

        private void OnEntityUpdate(EntityInfo entityInfo)
        {

            var message = new SimpleMessage()
            {
                MessageId = (int)ServerCodes.EntityInfo,

                Info = entityInfo.ToByteString(),
            };

            Console.WriteLine("Sending Entity Info" );
            SendArea(entityInfo.Position, message);
        }

        private void OnEntityUpdatePartial(EntityUpdate entityUpdate)
        {

            var entity = _entities.GetEntity(entityUpdate.EntityId);

            var message = new SimpleMessage()
            {
                MessageId = (int)ServerCodes.EntityUpdate,

                Info = entityUpdate.ToByteString(),
            };

            Console.WriteLine("Sending Entity Update");
            SendArea(entity.Position, message);
        }

    }
}
