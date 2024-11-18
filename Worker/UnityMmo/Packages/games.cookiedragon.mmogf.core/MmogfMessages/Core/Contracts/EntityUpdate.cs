using Mmogf.Servers.Shared;
using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct EntityUpdate
    {
        [Key(0)]
        public EntityId EntityId { get; set; }
        [Key(1)]
        public short ComponentId { get; set; }
        [Key(2)]
        public byte[] Info { get; set; }
    }
}