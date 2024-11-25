using System.Runtime.Serialization;

namespace Mmogf.Core.Contracts.Commands
{
    [DataContract]
    public struct CommandRequest
    {
        [DataMember(Order = 1)]
        public CommandRequestHeader Header { get; set; }

        [DataMember(Order = 2)]
        public byte[] Payload { get; set; }

        public CommandRequest(CommandRequestHeader header, byte[] payload)
        {

            Header = header;
            Payload = payload;
        }
    }

    [DataContract]
    public struct CommandRequest<T>
    {
        [DataMember(Order = 1)]
        public CommandRequestHeader Header { get; set; }

        [DataMember(Order = 2)]
        public T Payload { get; set; }

        public CommandRequest(CommandRequestHeader header, T payload)
        {

            Header = header;
            Payload = payload;
        }
    }
}
