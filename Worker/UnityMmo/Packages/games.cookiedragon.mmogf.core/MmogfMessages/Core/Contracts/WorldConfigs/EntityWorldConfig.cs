using Mmogf.Servers.Shared;
using System.Collections.Generic;
using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct EntityWorldConfig
    {
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public EntityId EntityId { get; set; }
        [Key(2)]
        public Dictionary<short, byte[]> EntityData { get; set; }


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

    }
}