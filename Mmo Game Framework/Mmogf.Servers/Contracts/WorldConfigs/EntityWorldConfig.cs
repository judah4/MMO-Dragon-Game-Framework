using MessagePack;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct EntityWorldConfig
    {
        [DataMember(Order = 0)]
        public string Name { get; set; }

        [DataMember(Order = 1)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 2)]
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

        public CreateEntityRequest ToCreateRequest()
        {

            Rotation? rotation = null;
            byte[] rotationBytes;
            if (EntityData.TryGetValue(Rotation.ComponentId, out rotationBytes))
            {
                rotation = MessagePackSerializer.Deserialize<Rotation>(rotationBytes);
            }

            var aclList = MessagePackSerializer.Deserialize<Acls>(EntityData[Acls.ComponentId]).AclList;

            var comps = new Dictionary<short, byte[]>();

            foreach (var comp in EntityData)
            {
                if (comp.Key == FixedVector3.ComponentId || comp.Key == Rotation.ComponentId || comp.Key == Acls.ComponentId)
                    continue;

                comps[comp.Key] = comp.Value;
            }

            var createEntity = new CreateEntityRequest(Name, MessagePackSerializer.Deserialize<FixedVector3>(EntityData[FixedVector3.ComponentId]), rotation ?? Rotation.Zero, comps, aclList);
            return createEntity;
        }

        public static EntityWorldConfig Create(string name, EntityId entityId, Position position, Rotation rotation, List<Acl> acls, Dictionary<short, IEntityComponent> additionalData)
        {

            var data = new Dictionary<short, byte[]>();

            data[FixedVector3.ComponentId] = MessagePack.MessagePackSerializer.Serialize(position.ToFixedVector3());
            data[Rotation.ComponentId] = MessagePack.MessagePackSerializer.Serialize(rotation);
            data[Acls.ComponentId] = MessagePack.MessagePackSerializer.Serialize(new Acls() { AclList = acls });

            if (additionalData != null)
            {
                foreach (var dat in additionalData)
                {
                    data[dat.Key] = MessagePack.MessagePackSerializer.Serialize(dat.Value.GetType(), dat.Value);
                }
            }

            var entityData = new EntityWorldConfig()
            {
                Name = name,
                EntityId = entityId,
                EntityData = data,
            };

            return entityData;
        }

    }
}