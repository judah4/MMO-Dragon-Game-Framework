using MessagePack;
using Microsoft.Extensions.Logging;
using Mmogf.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using static Grpc.Core.Metadata;

namespace MmoGameFramework
{
    public class EntityStore
    {
        private int lastId = 0;

        private ConcurrentDictionary<int, Entity> _entities = new ConcurrentDictionary<int, Entity>();

        ILogger _logger;

        public event Action<Entity> OnUpdateEntity;
        public event Action<CommandRequest> OnEntityCommand;
        public event Action<CommandResponse> OnEntityCommandResponse;
        public event Action<EntityUpdate, long> OnUpdateEntityPartial;
        public event Action<EventRequest> OnEntityEvent;
        public event Action<Entity> OnEntityDelete;

        public EntityStore(ILogger<EntityStore> logger)
        {
            _logger = logger;
        }

        public Entity Create(string entityType, Position position, Rotation rotation, List<Acl> acls, int? entityId = null, Dictionary<short, byte[]> additionalData = null)
        {
            if(entityId.HasValue)
            {
                if(lastId <= entityId.Value)
                    lastId = entityId.Value + 1; 
            }
            else
            {
                entityId = ++lastId;
            }

            //todo: Validate acl list for data passsed
            var data = new Dictionary<short, byte[]>()
            {
                { EntityType.ComponentId, MessagePackSerializer.Serialize(new EntityType() { Name = entityType}) },
                { FixedVector3.ComponentId, MessagePackSerializer.Serialize(position.ToFixedVector3()) },
                { Rotation.ComponentId, MessagePackSerializer.Serialize(rotation) },
                { Acls.ComponentId, MessagePackSerializer.Serialize(new Acls() { AclList = acls }) },
            };
            if (additionalData != null)
            {
                foreach (var additional in additionalData)
                {
                    if (data.ContainsKey(additional.Key))
                        continue;

                    data.Add(additional.Key, additional.Value);
                }
            }

            var entity = new Entity(entityId.Value, data);

            _entities.TryAdd(entityId.Value, entity);
            return entity;
        }

        public List<Entity> GetInArea(Position position, float radius)
        {
            var entities = new List<Entity>();
            foreach (var entityInfo in _entities)
            {
                if (Position.WithinArea(entityInfo.Value.Position, position, radius))
                {
                    entities.Add(entityInfo.Value);
                }
            }

            return entities;
        }

        public Entity? GetEntity(int entityId)
        {
            Entity entityInfo;
            if(!_entities.TryGetValue(entityId, out entityInfo))
                return null;

            return entityInfo;        
        }

        public void UpdateEntity(Entity entity)
        {
            _entities[entity.EntityId] = entity;
            OnUpdateEntity?.Invoke(entity);
        }

        public void UpdateEntityPartial(Entity entity, EntityUpdate entityUpdate, long workerId)
        {
            _entities[entity.EntityId] = entity;
            OnUpdateEntityPartial?.Invoke(entityUpdate, workerId);
        }

        public void SendCommand(CommandRequest commandRequest)
        {
            OnEntityCommand?.Invoke(commandRequest);
        }

        public void SendCommandResponse(CommandResponse commandResponse)
        {
            OnEntityCommandResponse?.Invoke(commandResponse);
        }

        public void SendEvent(EventRequest eventRequest)
        {
            OnEntityEvent?.Invoke(eventRequest);
        }

        public void Delete(int entityId)
        {
            Entity entity;
            if(!_entities.Remove(entityId, out entity))
                return;

            OnEntityDelete?.Invoke(entity);
        }
    }
}
