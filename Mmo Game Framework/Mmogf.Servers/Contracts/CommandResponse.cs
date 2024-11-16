using System.Runtime.Serialization;
namespace Mmogf.Servers.Contracts
{
    public enum CommandStatus
    {
        Failure,
        Success,
        InvalidRequest,
        Timeout,
    }

    [DataContract]
    public struct CommandResponse
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
        [DataMember(Order = 7)]
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