using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct ServerConnect
    {
        [Key(0)]
        public long ServerId { get; set; }
    }
}