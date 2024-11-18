using MessagePack;

namespace Mmogf.Core.Contracts.Commands
{
    [MessagePackObject]
    public struct CommandRequest
    {
        [Key(0)]
        public CommandRequestHeader Header { get; set; }

        [Key(1)]
        public byte[] Payload { get; set; }

        public CommandRequest(CommandRequestHeader header, byte[] payload)
        {

            Header = header;
            Payload = payload;
        }
    }
}
