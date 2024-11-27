using MmoGameFramework;
using Mmogf.Core.Contracts;
using Mmogf.Servers.Operations;

namespace Mmogf.Servers.Handlers.WorldCommands
{
    public class DeleteEntityCommandRequestHandler
    {
        public short CommandId => World.DeleteEntity.CommandId;

        private readonly DeleteEntityOperation _deleteEntityOperation;

        public DeleteEntityCommandRequestHandler(DeleteEntityOperation createEntityOperation)
        {
            _deleteEntityOperation = createEntityOperation;
        }

        public ImmutableEntity Handle(DeleteEntityRequest request)
        {
            var entity = _deleteEntityOperation.Execute(request);
            return entity;
        }
    }
}
