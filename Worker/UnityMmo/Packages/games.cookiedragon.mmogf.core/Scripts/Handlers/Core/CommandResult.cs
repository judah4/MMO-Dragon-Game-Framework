using Mmogf.Core.Contracts;
using Mmogf.Core.Contracts.Commands;
using Mmogf.Servers.Shared;

namespace Mmogf.Core
{
    public struct CommandResult<TCommand, TRequest, TResponse> where TCommand : ICommandBase<TRequest, TResponse> where TRequest : struct where TResponse : struct
    {
        public string RequestId { get; set; }

        public CommandStatus CommandStatus { get; set; }

        public string Message { get; set; }

        public long RequesterId { get; set; }

        public EntityId EntityId { get; set; }
        public int ComponentId { get; set; }
        public int CommandId { get; set; }

        public TRequest? Request { get; set; }
        public TResponse? Response { get; set; }

        public static CommandResult<TCommand, TRequest, TResponse> Create(CommandResponse response, TRequest? requestPayload, TResponse? responsePayload)
        {
            return new CommandResult<TCommand, TRequest, TResponse>()
            {
                RequestId = response.Header.RequestId,
                CommandStatus = response.Header.CommandStatus,
                Message = response.Header.Message,
                RequesterId = response.Header.RequesterId,
                EntityId = response.Header.EntityId,
                ComponentId = response.Header.ComponentId,
                CommandId = response.Header.CommandId,
                Request = requestPayload,
                Response = responsePayload,
            };
        }
    }
}
