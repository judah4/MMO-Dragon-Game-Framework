using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct MmoMessage
    {
        [Key(0)]
        public ServerCodes MessageId { get; set; }
        [Key(1)]
        public byte[] Info { get; set; }
    }
}