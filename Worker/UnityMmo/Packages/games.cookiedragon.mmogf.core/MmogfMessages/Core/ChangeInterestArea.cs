using MessagePack;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct ChangeInterestArea : IMessage
    {
        [Key(0)]
        public FixedVector3 Position { get; set; }

    }
}