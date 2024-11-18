using MessagePack;

namespace Mmogf.Core.Contracts.Commands
{
    [MessagePackObject]
    public struct CommandResponse
    {
        [Key(0)]
        public CommandResponseHeader Header { get; set; }

        [Key(1)]
        public byte[] Payload { get; set; }

        public CommandResponse(CommandResponseHeader header, byte[] payload)
        {

            Header = header;
            Payload = payload;
        }
    }
}
