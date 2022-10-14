using MessagePack;
using Microsoft.Extensions.Logging;
using Mmogf.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MmoGameFramework
{
    public struct Entity
    {

        public int EntityId { get; set; }
        public Dictionary<short, byte[]> EntityData { get; set; }
        public EntityType EntityType { get; private set; }
        public Position Position { get; private set; }

        public Rotation Rotation { get; private set; }

        public Acls Acls { get; private set; }

        public Entity(int entityId, Dictionary<short, byte[]> data) : this()
        {
            EntityId = entityId;
            EntityData = data;

            UpdateComponent(EntityType.ComponentId, EntityData[EntityType.ComponentId]);
            UpdateComponent(FixedVector3.ComponentId, EntityData[FixedVector3.ComponentId]);
            UpdateComponent(Rotation.ComponentId, EntityData[Rotation.ComponentId]);
            UpdateComponent(Acls.ComponentId, EntityData[Acls.ComponentId]);
        }

        public void UpdateComponent(short componentId, byte[] data)
        {
            EntityData[componentId] = data;

            switch (componentId)
            {
                case EntityType.ComponentId:
                    EntityType = MessagePackSerializer.Deserialize<EntityType>(EntityData[EntityType.ComponentId]);
                    break;
                case FixedVector3.ComponentId:
                    Position = MessagePackSerializer.Deserialize<FixedVector3>(EntityData[FixedVector3.ComponentId]).ToPosition();
                break;
                case Rotation.ComponentId:
                    Rotation = MessagePackSerializer.Deserialize<Rotation>(EntityData[Rotation.ComponentId]);
                    break;
                case Acls.ComponentId:
                    Acls = MessagePackSerializer.Deserialize<Acls>(EntityData[Acls.ComponentId]);
                    break;

            }
        }

        public EntityInfo ToEntityInfo()
        {
            var info = new EntityInfo()
            {
                EntityId = EntityId,
                EntityData = EntityData,
            };

            return info;

        }

    }
}
