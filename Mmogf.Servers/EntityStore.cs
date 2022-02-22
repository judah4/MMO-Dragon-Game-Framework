using MessagePack;
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

        public event Action<EntityInfo> OnUpdateEntity;
        public event Action<CommandRequest> OnEntityCommand;
        public event Action<CommandResponse> OnEntityCommandResponse;
        public event Action<EntityUpdate> OnUpdateEntityPartial;

        public EntityInfo Create(string entityType, Position position, Rotation? rotation = null, Dictionary<int, byte[]> additionalData = null)
        {
            var entityId = ++lastId;

            if(rotation == null)
                rotation = Rotation.Identity;

            var entity = new EntityInfo()
            {
                EntityId = entityId,
                EntityData = new Dictionary<int, byte[]>()
                {
                    { EntityType.ComponentId, MessagePackSerializer.Serialize(new EntityType() { Name = entityType}) },
                    { Position.ComponentId, MessagePackSerializer.Serialize(position) },
                    { Rotation.ComponentId, MessagePackSerializer.Serialize(rotation) }
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

            _entities.Add(entityId, entity);
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

        public void UpdateEntityPartial(EntityUpdate entityUpdate)
        {
            OnUpdateEntityPartial?.Invoke(entityUpdate);
        }

        public void SendCommand(CommandRequest commandRequest)
        {
            OnEntityCommand?.Invoke(commandRequest);
        }

        public void SendCommandResponse(CommandResponse commandResponse)
        {
            OnEntityCommandResponse?.Invoke(commandResponse);
        }
    }
}
