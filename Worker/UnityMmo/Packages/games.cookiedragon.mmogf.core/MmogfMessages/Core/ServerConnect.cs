using MessagePack;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct ServerConnect : IMessage
    {
        [Key(0)]
        public long ServerId { get; set; }
    }
}