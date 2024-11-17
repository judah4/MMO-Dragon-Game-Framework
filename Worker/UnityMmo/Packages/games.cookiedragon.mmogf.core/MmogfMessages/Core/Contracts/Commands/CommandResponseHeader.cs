using Mmogf.Servers.Shared;
using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts.Commands
{
    public enum CommandStatus
    {
        Failure,
        Success,
        InvalidRequest,
        Timeout,
    }

    [DataContract]
    public struct CommandResponseHeader
    {

        [DataMember(Order = 0)]
        public string RequestId { get; set; }

        [DataMember(Order = 1)]
        public CommandStatus CommandStatus { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public long RequesterId { get; set; }

        [DataMember(Order = 4)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 5)]
        public short ComponentId { get; set; }
        [DataMember(Order = 6)]
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