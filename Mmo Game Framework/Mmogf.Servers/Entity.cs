using Mmogf.Core.Contracts;
using Mmogf.Servers;
using Mmogf.Servers.Shared;
using System;
using System.Collections.Generic;

namespace MmoGameFramework
{
    public class Entity
    {
        public EntityId EntityId { get; }
        public Dictionary<short, IComponentData> AdditionalData { get; }
        public EntityType EntityType { get; }

        public Acls Acls { get; }

        public Position Position { get; private set; }

        public Rotation Rotation { get; private set; }


        public Entity(EntityId entityId, EntityType entityType, Acls acls, Position position, Rotation rotation, Dictionary<short, IComponentData> additionalData)
        {
            EntityId = entityId;
            EntityType = entityType;
            Acls = acls;
            Position = position;
            Rotation = rotation;
            AdditionalData = new Dictionary<short, IComponentData>();

            foreach (var item in additionalData)
            {
                UpdateComponent(item.Key, item.Value);
            }
        }

        public void UpdateComponent(short componentId, IComponentData data)
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

            AdditionalData[componentId] = data;
        }

        public void UpdatePosition(Position position)
        {
            Position = position;
        }

        public void UpdateRotation(Rotation rotation)
        {
            Rotation = rotation;
        }
    }
}
