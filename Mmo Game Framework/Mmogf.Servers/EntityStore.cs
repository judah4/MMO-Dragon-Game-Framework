using MessagePack;
using Microsoft.Extensions.Logging;
using Mmogf.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MmoGameFramework
{
    public class EntityStore
    {
        private int lastId = 0;

        private Dictionary<int, EntityInfo> _entities = new Dictionary<int, EntityInfo>();

        ILogger _logger;

        public event Action<EntityInfo> OnUpdateEntity;
        public event Action<CommandRequest> OnEntityCommand;
        public event Action<CommandResponse> OnEntityCommandResponse;
        public event Action<EntityUpdate, long> OnUpdateEntityPartial;
        public event Action<EventRequest> OnEntityEvent;
        public event Action<EntityInfo> OnEntityDelete;

        public EntityStore(ILogger<EntityStore> logger)
        {
            _logger = logger;
        }

        public EntityInfo Create(string entityType, Position position, List<Acl> acls, int? entityId = null, Rotation? rotation = null, Dictionary<int, byte[]> additionalData = null)
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


            if (rotation == null)
                rotation = Rotation.Zero;

            //validate acl list for data passsed

            var entity = new EntityInfo()
            {
                EntityId = entityId.Value,
                EntityData = new Dictionary<int, byte[]>()
                {
                    { EntityType.ComponentId, MessagePackSerializer.Serialize(new EntityType() { Name = entityType}) },
                    { Position.ComponentId, MessagePackSerializer.Serialize(position) },
                    { Rotation.ComponentId, MessagePackSerializer.Serialize(rotation) },
                    { Acls.ComponentId, MessagePackSerializer.Serialize(new Acls() { AclList = acls }) },
                }
            };

            if(additionalData != null)
            {
                foreach(var additional in additionalData)
                {
                    if(entity.EntityData.ContainsKey(additional.Key))
                        continue;

                    entity.EntityData.Add(additional.Key, additional.Value);
                }
            }

            _entities.Add(entityId.Value, entity);
            return entity;
        }

        public List<EntityInfo> GetInArea(Position position, float radius)
        {
            var entities = new List<EntityInfo>();
            foreach (var entityInfo in _entities)
            {
                if (Position.WithinArea(entityInfo.Value.Position, position, radius))
                {
                    entities.Add(entityInfo.Value);
                }
            }

            return entities;
        }

        public EntityInfo? GetEntity(int entityId)
        {
            EntityInfo entityInfo;
            if(!_entities.TryGetValue(entityId, out entityInfo))
                return null;

            return entityInfo;        
        }

        public void UpdateEntity(EntityInfo entityInfo)
        {
            OnUpdateEntity?.Invoke(entityInfo);
        }

        public void UpdateEntityPartial(EntityUpdate entityUpdate, long workerId)
        {
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
            var entity = GetEntity(entityId);
            if(entity == null)
                return;

            _entities.Remove(entityId);

            OnEntityDelete?.Invoke(entity.Value);
        }
    }
}
