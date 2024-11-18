using MessagePack;

namespace Mmogf.Core.Contracts.Events
{
    [MessagePackObject]
    public struct EventRequest
    {
        [Key(0)]
        public EventRequestHeader Header { get; set; }
        [Key(1)]
        public byte[] Payload { get; set; }
    }
}
