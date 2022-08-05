using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct EntityWorldConfig : IMessage
    {
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public int EntityId { get; set; }
        [Key(2)]
        public Dictionary<int, IEntityComponent> EntityData { get; set; }


        /*
         
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
         */

        public static EntityWorldConfig CreateConfig(string name, int entityId, Position position, List<Acl> acls, Dictionary<int, IEntityComponent> additionalData)
        {
            if(additionalData == null)
                additionalData = new Dictionary<int, IEntityComponent>();

            additionalData[Position.ComponentId] = position;
            additionalData[Acls.ComponentId] = new Acls() { AclList = acls };

            var entityData = new EntityWorldConfig()
            {
                Name = name,
                EntityId = entityId,
                EntityData = additionalData,
            };

            return entityData;
        }

    }
}