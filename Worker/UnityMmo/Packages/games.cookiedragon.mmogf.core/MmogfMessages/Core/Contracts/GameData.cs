using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct GameData
    {
        [Key(0)]
        public int EntityId { get; set; }
        [Key(1)]
        public byte[] Info { get; set; }

    }
}