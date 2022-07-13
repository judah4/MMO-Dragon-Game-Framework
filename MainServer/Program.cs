using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mmogf.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MmoGameFramework
{
    class Program
    {
        private static MmoServer server;
        private static MmoServer workerServer;
        private static EntityStore _entityStore;

        static Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder =>
                {
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    });
                })
                .Build();


            var logger = host.Services.GetRequiredService<ILogger<Program>>();

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

            var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";
            logger.LogInformation($"Dragon Game Framework MMO Networking Version {version}");
            logger.LogInformation("Attaching Entity Storage.");
            _entityStore = new EntityStore(host.Services.GetRequiredService<ILogger<EntityStore>>());

            logger.LogInformation("Creating PlayerCreator.");

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

            logger.LogDebug("Creating Test Cube.");
            //create starter objects
            _entityStore.Create("Cube", new Position() {X = 3, Z = 3}, new List<Acl>()
            {
                new Acl() { ComponentId = Position.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
            });

            logger.LogDebug("Creating Test Npc Spawner.");
            //create starter objects
            _entityStore.Create("NpcSpawner", new Position() { X = -3, Y = 0, Z = -25 }, new List<Acl>()
            {
                new Acl() { ComponentId = Position.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
            });

            logger.LogInformation("Starting Dragon-Client connections. Port 1337");
            // create and start the server
            server = new MmoServer(_entityStore, new NetPeerConfiguration("Dragon-Client")
            {
                MaximumConnections = 100,
                Port = 1337,
            }, host.Services.GetRequiredService<ILogger<MmoServer>>());
            server.Start();
            logger.LogInformation("Starting Dragon-Worker connections. Port 1338.");
            workerServer = new MmoServer(_entityStore, new NetPeerConfiguration("Dragon-Worker")
            {
                MaximumConnections = 100,
                Port = 1338,
            }, host.Services.GetRequiredService<ILogger<MmoServer>>());
            workerServer.Start();

            logger.LogInformation("DragonGF is ready.");
            bool loop = true;
            logger.LogInformation("ESC to close.");

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
                    logger.LogError(e, e.Message);
                }
               
            }

            server.Stop();
            workerServer.Stop();

            return host.RunAsync();

        }

    }
}
