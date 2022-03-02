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
            _entityStore.Create("Cube", new Position() {X = 3, Z = 3}, new List<Acl>()
            {
                new Acl() { ComponentId = Position.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
            });

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
            Console.WriteLine("ESC to close.");

            while (loop)
            {
                try
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
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
