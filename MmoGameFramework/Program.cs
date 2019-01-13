using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;
using MessageProtocols;
using MessageProtocols.Server;
using Mmogf.Core;


namespace MmoGameFramework
{
    class Program
    {
        private static Telepathy.Server server;
        private static Telepathy.Client client;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

           // create and start the server
           server = new Telepathy.Server();
           server.Start(1337);

           // create and connect the client
           client = new Telepathy.Client();
           client.Connect("localhost", 1337);

            var thread1 = new Thread(() => ServerLoop());
           thread1.Start();
           var thread2 = new Thread(() => ClientLoop());
           thread2.Start();

           bool loop = true;
           while (loop)
           {
               var key = Console.ReadKey();
               if (key.Key == ConsoleKey.Spacebar)
               {
                   var message = new SimpleMessage()
                   {
                       MessageId = (int)ServerCodes.GameData,
                       Info = new GameData()
                       {
                           EntityId = -1,
                           Info = ByteString.CopyFrom(0xFF, 0xFF),
                       }.ToByteString()
                   };
                    // send a message to server
                    client.Send(message.ToByteArray());
               }
               else if (key.Key == ConsoleKey.S)
               {

                   var message = new SimpleMessage()
                   {
                       MessageId = (int)ServerCodes.EntityInfo,
                       Info = new EntityInfo()
                       {
                           EntityId = 1,
                           EntityData = { new Dictionary<int, ByteString>()
                           {
                               {1, new Position() { X = 100, Y = 101, Z = 99}.ToByteString()},
                               {2, ByteString.CopyFrom(0x02)},
                               {3, ByteString.CopyFrom(0x03)},
                           }}

                       }.ToByteString(),
                   };
                    // send a message to server
                    server.Send(1, message.ToByteArray());

                }
                else if (key.Key == ConsoleKey.Escape)
               {
                   loop = false;
                   break;
               }
           }

           client.Disconnect();
           server.Stop();
        }

        static void ServerLoop()
        {

            while (server.Active)
            {
                // grab all new messages. do this in your Update loop.
                Telepathy.Message msg;
                while (server.GetNextMessage(out msg))
                {
                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            Console.WriteLine("Server "+ msg.connectionId + " Connected");
                            var message = new SimpleMessage()
                            {
                                MessageId = (int)ServerCodes.ClientConnect,
                                Info = new ClientConnect()
                                {
                                    ClientId = msg.connectionId,
                                }.ToByteString(),
                            };
                            server.Send(msg.connectionId, message.ToByteArray());
                            break;
                        case Telepathy.EventType.Data:
                            var simpleData = SimpleMessage.Parser.ParseFrom(msg.data);

                            Console.WriteLine("Server " + msg.connectionId);

                            switch ((ServerCodes)simpleData.MessageId)
                            {
                                case ServerCodes.GameData:
                                    var gameData = GameData.Parser.ParseFrom(simpleData.Info);
                                    Console.WriteLine($"Server Game Data: {BitConverter.ToString(gameData.Info.ToByteArray())}" );
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

        static void ClientLoop()
        {
            while (client.Connected || client.Connecting)
            {
                // grab all new messages. do this in your Update loop.
                Telepathy.Message msg;

                int clientId = -1;

                while (client.GetNextMessage(out msg))
                {
                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            Console.WriteLine("Client " + msg.connectionId + " Connected");
                            break;
                        case Telepathy.EventType.Data:
                            var simpleData = SimpleMessage.Parser.ParseFrom(msg.data);
                            Console.WriteLine("Client " + clientId + " Data: " + (ServerCodes)simpleData.MessageId);
                            switch ((ServerCodes)simpleData.MessageId)
                            {
                                case ServerCodes.ClientConnect:
                                    clientId = ClientConnect.Parser.ParseFrom(simpleData.Info).ClientId;
                                    Console.WriteLine("My Client Id is " + clientId);
                                    break;
                                case ServerCodes.GameData:
                                    var gameData = GameData.Parser.ParseFrom(simpleData.Info);
                                    Console.WriteLine($"Client Game Data: {BitConverter.ToString(gameData.Info.ToByteArray())}");
                                    break;
                                case ServerCodes.EntityInfo:
                                    var entityInfo = EntityInfo.Parser.ParseFrom(simpleData.Info);
                                    Console.WriteLine($"Client Entity Info: {entityInfo.EntityId}");
                                    foreach (var pair in entityInfo.EntityData)
                                    {
                                        if (pair.Key == 1)
                                        {
                                            var position = Position.Parser.ParseFrom(pair.Value);
                                            Console.WriteLine($"{pair.Key} Pos:{position.ToString()}");

                                        }
                                        else
                                        {
                                            Console.WriteLine($"{pair.Key} {BitConverter.ToString(pair.Value.ToByteArray())}");

                                        }

                                    }
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case Telepathy.EventType.Disconnected:
                            Console.WriteLine("Client " + msg.connectionId + " Disconnected");
                            break;
                    }
                }

                Thread.Sleep(10);

            }

        }

    }
}
