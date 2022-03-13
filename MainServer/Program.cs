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
            random = new Random(Environment.TickCount);

            if(random.Next(2) == 1)
            {
                Console.WriteLine(@"
                                   __ 
                                  / _|
  _ __ ___  _ __ ___   ___   __ _| |_ 
 | '_ ` _ \| '_ ` _ \ / _ \ / _` |  _|
 | | | | | | | | | | | (_) | (_| | |  
 |_| |_| |_|_| |_| |_|\___/ \__, |_|  
                             __/ |    
                            |___/     
");
            }
            else
            {
                Console.WriteLine(
                    @"


     _                             _____ ______ 
    | |                           |  __ \|  ___|
  __| |_ __ __ _  __ _  ___  _ __ | |  \/| |_   
 / _` | '__/ _` |/ _` |/ _ \| '_ \| | __ |  _|  
| (_| | | | (_| | (_| | (_) | | | | |_\ \| |    
 \__,_|_|  \__,_|\__, |\___/|_| |_|\____/\_|    
                  __/ |                         
                 |___/                          


");
            }
            

            Console.WriteLine("Attaching Entity Storage.");
            _entityStore = new EntityStore();

            Console.WriteLine("Creating PlayerCreator.");

            _entityStore.Create("PlayerCreator", new Position() { X = 0, Z = 0 }, new List<Acl>()
            {
                new Acl() { ComponentId = Position.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = PlayerCreator.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
            }, additionalData: new Dictionary<int, byte[]>()
            {
                { PlayerCreator.ComponentId, MessagePack.MessagePackSerializer.Serialize(new PlayerCreator() { }) },
            });

            Console.WriteLine("Creating Test Cube.");
            //create starter objects
            _entityStore.Create("Cube", new Position() {X = 3, Z = 3}, new List<Acl>()
            {
                new Acl() { ComponentId = Position.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
            });

            Console.WriteLine("Starting Dragon-Client connections. Port 1337");
            // create and start the server
            server = new MmoServer(_entityStore, new NetPeerConfiguration("Dragon-Client")
            {
                MaximumConnections = 100,
                Port = 1337,
            });
            server.Start();
            Console.WriteLine("Starting Dragon-Worker connections. Port 1338.");
            workerServer = new MmoServer(_entityStore, new NetPeerConfiguration("Dragon-Worker")
            {
                MaximumConnections = 100,
                Port = 1338,
            });
            workerServer.Start();

            Console.WriteLine("Dragongf/Mmogf is ready.");
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
