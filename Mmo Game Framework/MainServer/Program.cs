using Agones;
using Grpc.Core;
using Lidgren.Network;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mmogf.Core;
using Mmogf.Servers.Services;
//using Prometheus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MmoGameFramework
{
    class Program
    {
        private static MmoServer server;
        private static MmoServer workerServer;
        private static EntityStore _entityStore;

        static async Task Main(string[] args)
        {
            //var metricServer = new KestrelMetricServer(port: 1234);
            //metricServer.Start();

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
                .ConfigureServices(x =>
                {
                    x.AddSingleton<OrchestrationService>();
                })
                .Build();


            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var configuration = host.Services.GetRequiredService<IConfiguration>();
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

            logger.LogInformation("Loading World Configuration.");

            string worldFilePath = configuration.GetValue<string>("WorldFilePath") ?? "worlds/default.world";
            var updatedPath = Path.GetFullPath(worldFilePath);
            if(!File.Exists(updatedPath))
            {
                logger.LogError($"World file {updatedPath} does not exist!");
                await Task.Delay(1000); //delay so we can actually see the error...
                throw new Exception($"World file {updatedPath} does not exist!");
            }

            var worldBytes = File.ReadAllBytes(updatedPath);
            var worldData = MessagePack.MessagePackSerializer.Deserialize<WorldConfig>(worldBytes);

            foreach(var entity in worldData.Entities)
            {
                Rotation? rotation = null;
                byte[] rotationBytes;
                if(entity.EntityData.TryGetValue(Rotation.ComponentId, out rotationBytes))
                {
                    rotation = MessagePackSerializer.Deserialize<Rotation>(rotationBytes);
                }

                _entityStore.Create(entity.Name, MessagePackSerializer.Deserialize<Position>(entity.EntityData[Position.ComponentId]), MessagePackSerializer.Deserialize<Acls>(entity.EntityData[Acls.ComponentId]).AclList, entity.EntityId, rotation, entity.EntityData);

            }
            logger.LogInformation($"Loaded {worldData.Entities.Count} Entities.");


            var orchestationService = host.Services.GetRequiredService<OrchestrationService>();
            await orchestationService.ConnectAsync();

            var tickRate = configuration.GetValue<int>(key: "TickRate");
            logger.LogInformation($"Setting Server Tick Rate {tickRate}");

            logger.LogInformation("Starting Dragon-Client connections. Port 1337");
            // create and start the server
            server = new MmoServer(orchestationService, _entityStore, new NetPeerConfiguration("Dragon-Client")
            {
                MaximumConnections = 100,
                Port = 1337,
            }, true, host.Services.GetRequiredService<ILogger<MmoServer>>(), configuration);
            server.Start();
            logger.LogInformation("Starting Dragon-Worker connections. Port 1338.");
            workerServer = new MmoServer(orchestationService, _entityStore, new NetPeerConfiguration("Dragon-Worker")
            {
                MaximumConnections = 100,
                Port = 1338,
            }, false, host.Services.GetRequiredService<ILogger<MmoServer>>(), configuration);
            workerServer.Start();

            logger.LogInformation("DragonGF is ready.");

            await host.RunAsync();

            await orchestationService.ShutdownAsync();
            server.Stop();
            workerServer.Stop();
            //metricServer.Stop();
        }

    }
}
