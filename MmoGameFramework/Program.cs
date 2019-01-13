using System;
using System.Threading;
using Google.Protobuf;
using MessageProtocols;
using MessageProtocols.Server;


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
                       Info = null,
                   };
                    // send a message to server
                    client.Send(message.ToByteArray());
               }
               else if (key.Key == ConsoleKey.S)
               {

                   var message = new SimpleMessage()
                   {
                       Info = null
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
                                Info = new ClientConnect()
                                {
                                    ClientId = msg.connectionId,
                                }.ToByteString(),
                            };
                            server.Send(msg.connectionId, message.ToByteArray());
                            break;
                        case Telepathy.EventType.Data:
                            var simpleData = SimpleMessage.Parser.ParseFrom(msg.data);
                            Console.WriteLine("Server " + msg.connectionId + " Data: " + simpleData.Info);
                            
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

                int clientId;

                while (client.GetNextMessage(out msg))
                {
                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            Console.WriteLine("Client " + msg.connectionId + " Connected");
                            break;
                        case Telepathy.EventType.Data:
                            var simpleData = SimpleMessage.Parser.ParseFrom(msg.data);
                            Console.WriteLine("Client " + msg.connectionId + " Data: " + (ServerCodes)simpleData.MessageId);
                            switch ((ServerCodes)simpleData.MessageId)
                            {
                                default:
                                    break;
                                ServerCodes.ClientConnect:

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
