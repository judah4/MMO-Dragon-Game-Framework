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
using System.Linq;
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
                
            // Print Banner
            Console.WriteLine(Resources.Data.Banner);

            var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";
            logger.LogInformation($"Dragon Game Framework MMO Networking Version {version}");
            int cellSize = configuration.GetValue<int?>("ChunkSize") ?? 50;
            logger.LogInformation($"Attaching Entity Storage. Cell Size {cellSize}.");
            _entityStore = new EntityStore(host.Services.GetRequiredService<ILogger<EntityStore>>(), cellSize);

            //var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
            //MessagePackSerializer.DefaultOptions = lz4Options;
            //rethink compressed by default

            logger.LogInformation("Loading World Configuration.");

            string worldFilePath = configuration.GetValue<string>("WorldFilePath") ?? "worlds/default.world";
            var updatedPath = Path.GetFullPath(worldFilePath);
            //Console.WriteLine($"{worldFilePath} {updatedPath}");
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
                Rotation rotation = Rotation.Zero;
                byte[] rotationBytes;
                if(entity.EntityData.TryGetValue(Rotation.ComponentId, out rotationBytes))
                {
                    rotation = MessagePackSerializer.Deserialize<Rotation>(rotationBytes);
                }

                _entityStore.Create(entity.Name, MessagePackSerializer.Deserialize<FixedVector3>(entity.EntityData[FixedVector3.ComponentId]).ToPosition(),
                    rotation, 
                    MessagePackSerializer.Deserialize<Acls>(entity.EntityData[Acls.ComponentId]).AclList, 
                    entity.EntityId, entity.EntityData);

            }
            logger.LogInformation($"Loaded {worldData.Entities.Count} Entities.");

            var orchestationService = host.Services.GetRequiredService<OrchestrationService>();
            await orchestationService.ConnectAsync();

            var tickRate = configuration.GetValue<int>("TickRate");
            var timeout = configuration.GetValue<int>("Timeout");
            if(timeout == 0)
                timeout = 25;
            logger.LogInformation($"Setting Server Tick Rate {tickRate}");
            if(timeout != 25)
                logger.LogInformation($"Setting Server Timeout To {timeout}");
            logger.LogInformation("Starting Dragon-Client connections. Port 1337");
            // create and start the server
            server = new MmoServer(orchestationService, _entityStore, new NetPeerConfiguration("Dragon-Client")
            {
                MaximumConnections = 100,
                Port = 1337,
                ConnectionTimeout = timeout,
            }, true, host.Services.GetRequiredService<ILogger<MmoServer>>(), configuration);
            server.Start();
            logger.LogInformation("Starting Dragon-Worker connections. Port 1338.");
            workerServer = new MmoServer(orchestationService, _entityStore, new NetPeerConfiguration("Dragon-Worker")
            {
                MaximumConnections = 100,
                Port = 1338,
                ConnectionTimeout = timeout,
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
