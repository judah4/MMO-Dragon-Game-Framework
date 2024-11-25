using System.Runtime.Serialization;

namespace Mmogf.Core.Contracts.Commands
{
    [DataContract]
    public struct CommandResponse
    {
        [DataMember(Order = 1)]
        public CommandResponseHeader Header { get; set; }

        [DataMember(Order = 2)]
        public byte[] Payload { get; set; }

        public CommandResponse(CommandResponseHeader header, byte[] payload)
        {
            Header = header;
            Payload = payload;
        }
    }

    [DataContract]
    public struct CommandResponse<T>
    {
        [DataMember(Order = 1)]
        public CommandResponseHeader Header { get; set; }

        [DataMember(Order = 2)]
        public T Payload { get; set; }

        public CommandResponse(CommandResponseHeader header, T payload)
        {
            Header = header;
            Payload = payload;
        }
    }
}
