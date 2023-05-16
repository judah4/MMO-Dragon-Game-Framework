using MessagePack;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct EntityUpdate : IMessage
    {
        [Key(0)]
        public EntityId EntityId { get; set; }
        [Key(1)]
        public short ComponentId { get; set; }
        [Key(2)]
        public byte[] Info { get; set; }
    }
}