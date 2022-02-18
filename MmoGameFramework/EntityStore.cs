using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using MessageProtocols.Core;
using MessageProtocols.Server;

namespace MmoGameFramework
{
    public class EntityStore
    {
        private int lastId = 0;

        private Dictionary<int, EntityInfo> _entities = new Dictionary<int, EntityInfo>();

        public event Action<EntityInfo> OnUpdateEntity;
        public event Action<EntityUpdate> OnUpdateEntityPartial;

        public EntityInfo Create(string entityType, Position position)
        {
            var entityId = ++lastId;
            var entity = new EntityInfo()
            {
                EntityId = entityId,
                EntityData =
                {
                    {1, new EntityType() { Name = entityType}.ToByteString()},
                    {Position.ComponentId, position.ToByteString()}
                }
            };

            _entities.Add(entityId, entity);
            return entity;
        }

        public List<EntityInfo> GetInArea(Position position, float radius)
        {
            var entities = new List<EntityInfo>();
            foreach (var entityInfo in _entities)
            {
                if (Position.WithinArea(Position.Parser.ParseFrom(entityInfo.Value.EntityData[Position.ComponentId]), position, radius))
                {
                    entities.Add(entityInfo.Value);
                }
            }

            return entities;
        }

        public EntityInfo GetEntity(int entityId)
        {
            EntityInfo entityInfo;
            _entities.TryGetValue(entityId, out entityInfo);

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
    }
}
