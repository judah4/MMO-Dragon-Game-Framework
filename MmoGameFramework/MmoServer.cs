using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Google.Protobuf;
using MessageProtocols;
using MessageProtocols.Server;

namespace MmoGameFramework
{
    public class MmoServer
    {
        private Telepathy.Server _server;

        public bool Active => _server.Active;

        private Dictionary<int, MmoServer> _peers = new Dictionary<int, MmoServer>();

        public MmoServer()
        {
            _server = new Telepathy.Server();
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
                // grab all new messages. do this in your Update loop.
                Telepathy.Message msg;
                while (_server.GetNextMessage(out msg))
                {
                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            Console.WriteLine("Server " + msg.connectionId + " Connected");
                            var message = new SimpleMessage()
                            {
                                MessageId = (int)ServerCodes.ClientConnect,
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

                            switch ((ServerCodes)simpleData.MessageId)
                            {
                                case ServerCodes.GameData:
                                    var gameData = GameData.Parser.ParseFrom(simpleData.Info);
                                    Console.WriteLine($"Server Game Data: {BitConverter.ToString(gameData.Info.ToByteArray())}");
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case Telepathy.EventType.Disconnected:
                            Console.WriteLine("Server " + msg.connectionId + " Disconnected");
                            break;
                    }
                }

                Thread.Sleep(10);
            }
        }

        public void SendClient(int clientId, IMessage message)
        {
            _server.Send(clientId, message.ToByteArray());
        }

        public void SendServer(int serverId, IMessage message)
        {

        }

    }
}
