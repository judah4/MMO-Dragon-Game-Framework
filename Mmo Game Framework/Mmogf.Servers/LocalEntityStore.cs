using MmoGameFramework;
using Mmogf.Core.Contracts;
using Mmogf.Servers.ServerInterfaces;
using Mmogf.Servers.Shared;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Mmogf.Servers
{
    public sealed class LocalEntityStore : IEntityStore
    {
        private readonly object _syncRoot = new object();
        private int lastId = 0;

        private readonly ConcurrentDictionary<EntityId, Entity> _entities = new ConcurrentDictionary<EntityId, Entity>();

        public Entity CreateEntity(string entityType, Position position, Rotation rotation, List<Acl> acls)
        {
            EntityId entityId;
            lock (_syncRoot)
            {
                entityId = new EntityId(++lastId);
            }

            var additionalData = new Dictionary<short, IComponentData>();

            var entityTypeData = new EntityType()
            {
                Name = entityType
            };

            var entity = new Entity(entityId, entityTypeData, new Acls() { AclList = acls }, position, rotation, additionalData);

            if (!_entities.TryAdd(entityId, entity))
            {
                throw new System.Exception($"Failed to create an {entityType} entity.");
            }

            return entity;
        }

        public Entity Delete(EntityId entityId)
        {
            Entity entityInfo;
            if (!_entities.TryRemove(entityId, out entityInfo))
            {
                throw new System.Exception($"Failed to delete entity {entityId}.");
            }

            return entityInfo;
        }

        public Entity Get(EntityId entityId)
        {
            Entity entityInfo;
            if (!_entities.TryGetValue(entityId, out entityInfo))
            {
                throw new System.Exception($"Failed to get entity {entityId}.");
            }

            return entityInfo;
        }

        public void Update(Entity entity)
        {
            _entities[entity.EntityId] = entity;
        }

        public Entity Update(EntityId entityId, short componentId, IComponentData data)
        {
            Entity entityInfo;
            if (!_entities.TryGetValue(entityId, out entityInfo))
            {
                throw new System.Exception($"Failed to get entity {entityId} to update.");
            }

            entityInfo.UpdateComponent(componentId, data);

            return entityInfo;
        }
    }
}
