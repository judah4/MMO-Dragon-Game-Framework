using Mmogf.Servers.Shared;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct EntityWorldConfig
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 3)]
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