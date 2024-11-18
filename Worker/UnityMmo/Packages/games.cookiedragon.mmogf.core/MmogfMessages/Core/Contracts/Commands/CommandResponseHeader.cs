using MessagePack;
using Mmogf.Servers.Shared;
namespace Mmogf.Core.Contracts.Commands
{
    public enum CommandStatus
    {
        Failure,
        Success,
        InvalidRequest,
        Timeout,
    }

    [MessagePackObject]
    public struct CommandResponseHeader
    {

        [Key(0)]
        public string RequestId { get; set; }

        [Key(1)]
        public CommandStatus CommandStatus { get; set; }

        [Key(2)]
        public string Message { get; set; }

        [Key(3)]
        public long RequesterId { get; set; }

        [Key(4)]
        public EntityId EntityId { get; set; }
        [Key(5)]
        public short ComponentId { get; set; }
        [Key(6)]
        public short CommandId { get; set; }

        public static CommandResponseHeader Create(CommandRequestHeader request, CommandStatus commandStatus, string message = "")
        {
            return new CommandResponseHeader()
            {
                RequestId = request.RequestId,
                CommandStatus = commandStatus,
                Message = message,
                RequesterId = request.RequesterId,
                EntityId = request.EntityId,
                ComponentId = request.ComponentId,
                CommandId = request.CommandId,
            };
        }

        public static CommandResponseHeader CreateError(CommandRequestHeader request, CommandStatus commandStatus, string message)
        {
            return new CommandResponseHeader()
            {
                RequestId = request.RequestId,
                CommandStatus = commandStatus,
                Message = message,
                RequesterId = request.RequesterId,
                EntityId = request.EntityId,
                ComponentId = request.ComponentId,
                CommandId = request.CommandId,
            };
        }
    }
}