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
        private static MmoServer server;
        private static MmoServer workerServer;
        private static EntityStore _entityStore;

        static void Main(string[] args)
        {
            _entityStore = new EntityStore();

           // create and start the server
           server = new MmoServer();
           server.Start(1337);
           workerServer = new MmoServer();
           workerServer.Start(1338);


           bool loop = true;
           while (loop)
           {
               var key = Console.ReadKey();
              if (key.Key == ConsoleKey.S)
               {

                   var message = new SimpleMessage()
                   {
                       MessageId = (int)ServerCodes.EntityInfo,

                       Info = _entityStore.Create().ToByteString(),
                   };
                    // send a message to clients. get lists
                    server.SendClient(1, message);

                }
                else if (key.Key == ConsoleKey.Escape)
               {
                   loop = false;
                   break;
               }
           }

           server.Stop();
        }


    }
}
