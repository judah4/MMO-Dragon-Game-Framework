using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using MessageProtocols.Server;
using Mmogf.Core;

namespace MmoGameFramework
{
    public class EntityStore
    {
        private int lastId = 0;

        public Dictionary<int, EntityInfo> _entities = new Dictionary<int, EntityInfo>();

        public EntityInfo Create()
        {
            var entityId = ++lastId;
            var entity = new EntityInfo()
            {
                EntityId = entityId,
                EntityData =
                {
                    {1, new EntityType() { Name = "Cube"}.ToByteString()},
                    {2, new Position() { X = 1, Y = 0, Z = 1}.ToByteString()}
                }
            };

            _entities.Add(entityId, entity);
            return entity;
        }

    }
}
