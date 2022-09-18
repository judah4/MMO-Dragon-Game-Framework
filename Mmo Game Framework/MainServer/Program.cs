﻿using Lidgren.Network;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mmogf.Core;
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

            logger.LogInformation("Loading World Configuration.");

            string worldFilePath = "worlds/default.world";
#if DEBUG
            //up and out of the debug bin file
            worldFilePath = "../../../../../../Worker/UnityMmo/worlds/default.world";
#endif
            var updatedPath = Path.GetFullPath(worldFilePath);
            if(!File.Exists(updatedPath))
            {
                logger.LogError($"World file {updatedPath} does not exist!");
                return Task.Delay(5000);
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

            while (loop)
            {
                //how do we want to quit?
                var task = Task.CompletedTask;
                task.Wait();
               
            }

            server.Stop();
            workerServer.Stop();

            return host.RunAsync();

        }

    }
}