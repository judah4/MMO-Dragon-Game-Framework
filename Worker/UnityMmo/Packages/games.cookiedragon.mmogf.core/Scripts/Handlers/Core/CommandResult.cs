using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Core
{
    public struct CommandResult<TCommand, TRequest, TResponse> where TCommand : ICommandBase<TRequest,TResponse> where TRequest : struct where TResponse : struct
    {
        public string RequestId { get; set; }

        public CommandStatus CommandStatus { get; set; }

        public string Message { get; set; }

        public long RequesterId { get; set; }

        public int EntityId { get; set; }
        public int ComponentId { get; set; }
        public int CommandId { get; set; }

        public TRequest? Request { get; set; }
        public TResponse? Response { get; set; }

        public static CommandResult<TCommand, TRequest, TResponse> Create(CommandResponse response, TRequest? requestPayload, TResponse? responsePayload)
        {
            return new CommandResult<TCommand, TRequest, TResponse>()
            {
                RequestId = response.RequestId,
                CommandStatus = response.CommandStatus,
                Message = response.Message,
                RequesterId = response.RequesterId,
                EntityId = response.EntityId,
                ComponentId = response.ComponentId,
                CommandId = response.CommandId,
                Request = requestPayload,
                Response = responsePayload,
            };
        }
    }
}
