using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    public enum CommandStatus
    {
        Failure,
        Success,
        InvalidRequest,
        Timeout,
    }

    [MessagePackObject]
    public struct CommandResponse : IMessage
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
        public int EntityId { get; set; }
        [Key(5)]
        public int ComponentId { get; set; }
        [Key(6)]
        public int CommandId { get; set; }
        [Key(7)]
        public byte[] Payload { get; set; }

        public static CommandResponse Create(CommandRequest request, CommandStatus commandStatus = CommandStatus.Success, string message = "", byte[] payload = null)
        {
            return new CommandResponse()
            {
                RequestId = request.RequestId,
                CommandStatus = commandStatus,
                Message = message,
                RequesterId = request.RequesterId,
                EntityId = request.EntityId,
                ComponentId = request.ComponentId,
                CommandId = request.CommandId,
                Payload = payload,
            };
        }
        

    }
}