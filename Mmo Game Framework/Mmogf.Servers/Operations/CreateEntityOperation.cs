using MmoGameFramework;
using Mmogf.Core.Contracts;
using Mmogf.Servers.ServerInterfaces;

namespace Mmogf.Servers.Operations
{
    public sealed class CreateEntityOperation
    {
        private readonly IEntityStore _entities;

        public CreateEntityOperation(IEntityStore entities)
        {
            _entities = entities;
        }

        public Entity Execute(CreateEntityRequest request)
        {
            var entityInfo = _entities.CreateEntity(request.EntityType, request.Position.ToPosition(), request.Rotation, request.Acls);
            return entityInfo;
        }
    }
}
