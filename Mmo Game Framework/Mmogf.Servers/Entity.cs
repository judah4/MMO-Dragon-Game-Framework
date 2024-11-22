using Mmogf.Core.Contracts;
using Mmogf.Servers.Serializers;
using Mmogf.Servers.Shared;
using System.Collections.Generic;

namespace MmoGameFramework
{
    public struct Entity
    {

        public EntityId EntityId { get; set; }
        public Dictionary<short, byte[]> EntityData { get; set; }
        public EntityType EntityType { get; private set; }
        public Position Position { get; private set; }

        public Rotation Rotation { get; private set; }

        public Acls Acls { get; private set; }

        private ISerializer _serializer;

        public Entity(EntityId entityId, Dictionary<short, byte[]> data, ISerializer serializer) : this()
        {
            EntityId = entityId;
            EntityData = data;
            _serializer = serializer;

            UpdateComponent(EntityType.ComponentId, EntityData[EntityType.ComponentId]);
            UpdateComponent(FixedVector3.ComponentId, EntityData[FixedVector3.ComponentId]);
            UpdateComponent(Rotation.ComponentId, EntityData[Rotation.ComponentId]);
            UpdateComponent(Acls.ComponentId, EntityData[Acls.ComponentId]);
        }

        public void UpdateComponent(short componentId, byte[] data)
        {
            EntityData[componentId] = data;

            // Serializer needs to fixed here
            switch (componentId)
            {
                case EntityType.ComponentId:
                    EntityType = _serializer.Deserialize<EntityType>(EntityData[EntityType.ComponentId]);
                    break;
                case FixedVector3.ComponentId:
                    Position = _serializer.Deserialize<FixedVector3>(EntityData[FixedVector3.ComponentId]).ToPosition();
                    break;
                case Rotation.ComponentId:
                    Rotation = _serializer.Deserialize<Rotation>(EntityData[Rotation.ComponentId]);
                    break;
                case Acls.ComponentId:
                    Acls = _serializer.Deserialize<Acls>(EntityData[Acls.ComponentId]);
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
