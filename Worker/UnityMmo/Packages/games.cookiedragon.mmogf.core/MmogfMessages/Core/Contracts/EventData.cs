using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct EventData
    {
        [Key(1)]
        public int EntityId { get; set; }
        [Key(2)]
        public short ComponentId { get; set; }
        [Key(3)]
        public byte[] Payload { get; set; }
    }
}