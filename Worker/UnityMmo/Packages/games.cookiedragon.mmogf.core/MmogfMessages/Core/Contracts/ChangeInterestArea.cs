using MessagePack;

namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct ChangeInterestArea
    {
        [Key(0)]
        public FixedVector3 Position { get; set; }

    }
}