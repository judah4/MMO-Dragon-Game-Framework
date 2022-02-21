using Lidgren.Network;
using Mmogf.Core;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MmoGameFramework
{
    class Program
    {
        private static MmoServer server;
        private static MmoServer workerServer;
        private static EntityStore _entityStore;

        private static Random random;

        static void Main(string[] args)
        {
            random = new Random();
            _entityStore = new EntityStore();

            //create starter objects
            _entityStore.Create("Cube", new Position() {X = 3, Z = 3});
            _entityStore.Create("Cube", new Position() { X = 1, Z = 3 });
            _entityStore.Create("Cube", new Position() { X = -3, Z = -3 });

            // create and start the server
            server = new MmoServer(_entityStore, new NetPeerConfiguration("Dragon-Client")
            {
                MaximumConnections = 100,
                Port = 1337,
            });
            server.Start();

            workerServer = new MmoServer(_entityStore, new NetPeerConfiguration("Dragon-Worker")
            {
                MaximumConnections = 100,
                Port = 1338,
            });
            workerServer.Start();

            bool loop = true;
            Console.WriteLine("S to spawn Cube.");
            Console.WriteLine("ESC to close.");

            while (loop)
            {
                try
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.S)
                    {
                        var newEntity = _entityStore.Create("Cube",
                            new Position() { X = random.NextDouble() * 10 - 5, Y = 2, Z = random.NextDouble() * 10 - 5 });

                        _entityStore.UpdateEntity(newEntity);

                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        loop = false;
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
               
            }

            server.Stop();
            workerServer.Stop();
        }


    }
}
