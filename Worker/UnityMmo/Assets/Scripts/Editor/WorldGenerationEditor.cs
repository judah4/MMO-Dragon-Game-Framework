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

        [MenuItem("DragonGF/Generate World Config", false, 200)]
        private static void Generate()
        {

            var playerCreatorEntity = EntityWorldConfig.CreateConfig("PlayerCreator", 1, new Position() { X = 0, Z = 0 }, new List<Acl>()
            {
                new Acl() { ComponentId = Position.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = PlayerCreator.ComponentId, WorkerType = "Dragon-Worker" },
                new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
            }, additionalData: new Dictionary<int, IEntityComponent>()
            {
                { PlayerCreator.ComponentId, new PlayerCreator() { } },
            });



            var path = Path.GetDirectoryName(WorldGenPath);
            Directory.CreateDirectory(path);
            File.Delete(WorldGenPath);

            using(var writer =  File.Create(WorldGenPath))
            {

                MessagePack.MessagePackSerializer.Serialize(writer, playerCreatorEntity);

                writer.Close();
            }

            Debug.Log($"Successfully generated world config at {WorldGenPath}");


        }
    }
}
