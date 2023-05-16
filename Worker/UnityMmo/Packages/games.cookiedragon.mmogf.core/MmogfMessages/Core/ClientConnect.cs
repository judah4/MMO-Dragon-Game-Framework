using MessagePack;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct ClientConnect : IMessage
    {
        [Key(0)]
        public long ClientId { get; set; }

    }
}