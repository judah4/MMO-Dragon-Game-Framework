using MessagePack;

namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct ClientConnect
    {
        [Key(0)]
        public long ClientId { get; set; }

    }
}