using MessagePack;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct GameData : IMessage
    {
        [Key(0)]
        public int EntityId { get; set; }
        [Key(1)]
        public byte[] Info { get; set; }

    }
}