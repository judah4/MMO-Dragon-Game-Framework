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

        static void Main(string[] args)
        {

           // create and start the server
           server = new MmoServer();
           server.Start(1337);


           bool loop = true;
           while (loop)
           {
               var key = Console.ReadKey();
              if (key.Key == ConsoleKey.S)
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
