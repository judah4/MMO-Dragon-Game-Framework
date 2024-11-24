using Mmogf.Core.Contracts;
using Mmogf.Servers;
using Mmogf.Servers.Shared;
using System;
using System.Collections.Generic;

namespace MmoGameFramework
{
    public struct ImmutableEntity
    {
        public static ImmutableEntity FromEntity(Entity entity)
        {
            return new ImmutableEntity(entity.EntityId, entity.EntityType, entity.Acls, entity.Position, entity.Rotation, entity.AdditionalData);
        }

        public EntityId EntityId { get; }
        public IReadOnlyDictionary<short, IComponentData> AdditionalData { get; }
        public EntityType EntityType { get; }

        public Acls Acls { get; }

        public Position Position { get; }

        public Rotation Rotation { get; }

        public ImmutableEntity(EntityId entityId, EntityType entityType, Acls acls, Position position, Rotation rotation, IReadOnlyDictionary<short, IComponentData> additionalData)
        {
            EntityId = entityId;
            EntityType = entityType;
            Acls = acls;
            Position = position;
            Rotation = rotation;
            var data = new Dictionary<short, IComponentData>();

            foreach (var item in additionalData)
            {
                UpdateComponent(data, item.Key, item.Value);
            }

            AdditionalData = data;
        }

        private void UpdateComponent(Dictionary<short, IComponentData> additionalData, short componentId, IComponentData data)
        {
            switch (componentId)
            {
                case EntityType.ComponentId:
                    throw new ArgumentException("Entity Type cannot be updated.", nameof(componentId));
                case FixedVector3.ComponentId:
                    throw new ArgumentException("Use Update Position instead.", nameof(componentId));
                case Rotation.ComponentId:
                    throw new ArgumentException("Use Update Rotation instead.", nameof(componentId));
                case Acls.ComponentId:
                    throw new ArgumentException("Entity acls cannot be updated at the moment.", nameof(componentId));
            }

            additionalData[componentId] = data;
        }
    }
}
