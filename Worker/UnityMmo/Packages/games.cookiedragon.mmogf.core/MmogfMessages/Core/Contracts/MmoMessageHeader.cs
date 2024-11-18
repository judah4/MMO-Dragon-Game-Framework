using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct MmoMessageHeader
    {
        [Key(0)]
        public ServerCodes MessageId { get; set; }
    }
}