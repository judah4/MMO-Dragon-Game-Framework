using MmoGameFramework;
using Mmogf.Core.Contracts;
using Mmogf.Servers.Serializers;
using System.Collections.Generic;

namespace Mmogf.Servers.Converters
{
    public class EntityToContractConverter
    {
        private readonly ISerializer _serializer;

        public EntityToContractConverter(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public EntityInfo Convert(Entity entity)
        {
            var entityData = new Dictionary<short, byte[]>(entity.AdditionalData.Count + 5)
            {
                { EntityType.ComponentId, _serializer.Serialize(entity.EntityType) },
                { Acls.ComponentId, _serializer.Serialize(entity.Acls) },
                { FixedVector3.ComponentId, _serializer.Serialize(entity.Position.ToFixedVector3()) },
                { Rotation.ComponentId, _serializer.Serialize(entity.Rotation) },
            };

            foreach (var item in entity.AdditionalData)
            {
                entityData.Add(item.Key, item.Value.AsBytes());
            }

            // TODO: Move this somewhere to use the serializer
            var info = new EntityInfo()
            {
                EntityId = entity.EntityId,
                EntityData = entityData,
            };

            return info;
        }
    }
}
