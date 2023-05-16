using MessagePack;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct MmoMessage: IMessage 
    {
        [Key(0)]
        public ServerCodes MessageId { get; set; }
        [Key(1)]
        public byte[] Info { get; set; }
    }
}