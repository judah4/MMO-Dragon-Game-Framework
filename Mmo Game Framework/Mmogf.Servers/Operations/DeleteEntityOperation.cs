using MmoGameFramework;
using Mmogf.Core.Contracts;
using Mmogf.Servers.ServerInterfaces;

namespace Mmogf.Servers.Operations
{
    public sealed class DeleteEntityOperation
    {
        private readonly IEntityStore _entities;

        public DeleteEntityOperation(IEntityStore entities)
        {
            _entities = entities;
        }

        public ImmutableEntity Execute(DeleteEntityRequest request)
        {
            var entityInfo = _entities.Delete(request.EntityId);
            return entityInfo;
        }
    }
}
