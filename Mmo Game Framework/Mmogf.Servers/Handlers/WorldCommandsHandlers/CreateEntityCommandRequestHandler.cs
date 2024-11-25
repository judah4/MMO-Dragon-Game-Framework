using MmoGameFramework;
using Mmogf.Core.Contracts;
using Mmogf.Servers.Operations;
using Mmogf.Servers.Serializers;

namespace Mmogf.Servers.Handlers.WorldCommands
{
    public class CreateEntityCommandRequestHandler
    {
        public short CommandId => World.CreateEntity.CommandId;

        private readonly ISerializer _serializer;
        private readonly CreateEntityOperation _createEntityOperation;

        public CreateEntityCommandRequestHandler(ISerializer serializer, CreateEntityOperation createEntityOperation)
        {
            _serializer = serializer;
            _createEntityOperation = createEntityOperation;
        }

        public ImmutableEntity Handle(CreateEntityRequest request)
        {
            var entity = _createEntityOperation.Execute(request);
            return entity;
        }
    }
}
