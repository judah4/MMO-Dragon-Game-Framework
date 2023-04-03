using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Mmogf
{
    public static class WorldGenerationEditor
    {
        private static readonly string WorldGenPath = Application.dataPath + "/../worlds/default.world";

        [MenuItem("DragonGF/Generate World Config", priority = 1200)]
        private static void Generate()
        {
            GenFile(false);
        }

        [MenuItem("DragonGF/Generate World Config (Player Only)", priority = 1200)]
        private static void GeneratePlayerOnly()
        {
            GenFile(true);
        }

        private static void GenFile(bool playerOnly)
        {
            MmogfStartup.RegisterSerializers();

            var worldConfig = new WorldConfig()
            {
                Version = "1",
                Entities = new List<EntityWorldConfig>()
                {
                }
            };

            var playerCreatorEntity = EntityWorldConfig.Create("PlayerCreator", new EntityId(1), new Position() { X = 0, Z = 0 }, Rotation.Zero, new List<Acl>()
            {
                new Acl() { ComponentId = FixedVector3.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = PlayerCreator.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
            }, new Dictionary<short, IEntityComponent>()
            {
                { PlayerCreator.ComponentId, new PlayerCreator() { } },
            });
            worldConfig.Entities.Add(playerCreatorEntity);

            if(playerOnly == false)
            {

                worldConfig.Entities.Add(EntityWorldConfig.Create("NpcSpawner", new EntityId(2), new Position() { X = -3, Y = 0, Z = -25 }, Rotation.Zero, new List<Acl>()
                {
                    new Acl() { ComponentId = FixedVector3.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
                }, new Dictionary<short, IEntityComponent>()));

                worldConfig.Entities.Add(EntityWorldConfig.Create("Cube", new EntityId(3), new Position() { X = 3, Z = 3 }, Rotation.Zero, new List<Acl>()
                {
                    new Acl() { ComponentId = FixedVector3.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
                }, new Dictionary<short, IEntityComponent>()));

                var entityId = 4;

                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        worldConfig.Entities.Add(EntityWorldConfig.Create("Ship", new EntityId(entityId++), new Position() { X = x * 10 - 50, Z = y * 10 - 50 }, Quaternion.Euler(0, Random.Range(-90, 90), 0).ToRotation(), new List<Acl>()
                        {
                            new Acl() { ComponentId = FixedVector3.ComponentId, WorkerType = "Dragon-Worker" },
                            new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                            new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
                            new Acl() { ComponentId = Cannon.ComponentId, WorkerType = "Dragon-Worker" },
                            new Acl() { ComponentId = Health.ComponentId, WorkerType = "Dragon-Worker" },
                        }, new Dictionary<short, IEntityComponent>()
                        {
                            { Cannon.ComponentId, new Cannon() },
                            { Health.ComponentId, new Health() { Current = 100, Max = 100, } },
                        }));
                    }
                }
            }

            var path = Path.GetDirectoryName(WorldGenPath);
            Directory.CreateDirectory(path);
            File.Delete(WorldGenPath);

            using(var writer =  File.Create(WorldGenPath))
            {

                MessagePack.MessagePackSerializer.Serialize(writer, worldConfig);

                writer.Close();
            }

            Debug.Log($"Successfully generated world config at {WorldGenPath} with {worldConfig.Entities.Count} entities");


        }
    }
}
