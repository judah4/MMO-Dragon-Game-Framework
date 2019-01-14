using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;
using MessageProtocols;
using MessageProtocols.Core;
using MessageProtocols.Server;

namespace MmoGameFramework
{
    public class MmoServer
    {
        private Telepathy.Server _server;
        private EntityStore _entities;

        public bool Active => _server.Active;

        public Dictionary<int, WorkerConnection> _connections = new Dictionary<int, WorkerConnection>();

        public MmoServer(EntityStore entities)
        {
            _entities = entities;
            _server = new Telepathy.Server();

            _entities.OnUpdateEntity += OnEntityUpdate;

        }

        public void Start(short port)
        {
            _server.Start(port);

            var thread1 = new Thread(Loop);
            thread1.Start();

        }

        public void Stop()
        {
            //send stop to peers
            _server.Stop();
        }

        void Loop()
        {
            while (_server.Active)
            {
                try
                {


                    // grab all new messages. do this in your Update loop.
                    Telepathy.Message msg;
                    while (_server.GetNextMessage(out msg))
                    {
                        switch (msg.eventType)
                        {
                            case Telepathy.EventType.Connected:
                                _connections.Add(msg.connectionId, new WorkerConnection("Worker", new Position()));
                                Console.WriteLine("Server " + msg.connectionId + " Connected");
                                var message = new SimpleMessage()
                                {
                                    MessageId = (int) ServerCodes.ClientConnect,
                                    Info = new ClientConnect()
                                    {
                                        ClientId = msg.connectionId,
                                    }.ToByteString(),
                                };
                                _server.Send(msg.connectionId, message.ToByteArray());
                                break;
                            case Telepathy.EventType.Data:
                                var simpleData = SimpleMessage.Parser.ParseFrom(msg.data);

                                Console.WriteLine("Server " + msg.connectionId);

                                switch ((ServerCodes) simpleData.MessageId)
                                {
                                    case ServerCodes.GameData:
                                        var gameData = GameData.Parser.ParseFrom(simpleData.Info);
                                        Console.WriteLine(
                                            $"Server Game Data: {BitConverter.ToString(gameData.Info.ToByteArray())}");
                                        break;
                                    case ServerCodes.ChangeInterestArea:
                                        var interestArea = ChangeInterestArea.Parser.ParseFrom(simpleData.Info);
                                        WorkerConnection worker;
                                        if (_connections.TryGetValue(msg.connectionId, out worker))
                                        {
                                            worker.InterestPosition = interestArea.Position;

                                            var entities = _entities.GetInArea(worker.InterestPosition,
                                                worker.InterestRange);

                                            foreach (var entityInfo in entities)
                                            {
                                                Send(msg.connectionId, new SimpleMessage()
                                                {
                                                    MessageId = (int) ServerCodes.EntityInfo,
                                                    Info = new EntityInfo(entityInfo).ToByteString(),
                                                });
                                            }

                                        }

                                        break;
                                    case ServerCodes.EntityUpdate:
                                        HandleEntityUpdate(msg, simpleData);
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            case Telepathy.EventType.Disconnected:
                                Console.WriteLine("Server " + msg.connectionId + " Disconnected");
                                _connections.Remove(msg.connectionId);

                                break;
                        }
                    }

                    Thread.Sleep(10);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        void HandleEntityUpdate(Telepathy.Message msg, SimpleMessage simpleData)
        {
            var entityUpdate = EntityUpdate.Parser.ParseFrom(simpleData.Info);
            var entityInfo = _entities.GetEntity(entityUpdate.EntityId);

            if (entityInfo == null)
            {
                return;

            }

            entityInfo.EntityData.Remove(entityUpdate.ComponentId);
            entityInfo.EntityData.Add(entityUpdate.ComponentId, entityUpdate.Info);


            _entities.UpdateEntity(entityInfo);

        }

        public void Send(int clientId, IMessage message)
        {
            _server.Send(clientId, message.ToByteArray());
        }

        public void SendArea(Position position, IMessage message)
        {
            foreach (var workerConnection in _connections)
            {
                if (Position.WithinArea(position, workerConnection.Value.InterestPosition,
                    workerConnection.Value.InterestRange))
                {
                    _server.Send(workerConnection.Key, message.ToByteArray());

                }
            }

        }

        private void OnEntityUpdate(EntityInfo entityInfo)
        {

            var message = new SimpleMessage()
            {
                MessageId = (int)ServerCodes.EntityInfo,

                Info = entityInfo.ToByteString(),
            };

            Console.WriteLine("Sending Entity Update" );
            SendArea(entityInfo.Position, message);
        }

    }
}
