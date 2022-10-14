using MessagePack;
using Mmogf.Core;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct ChangeInterestArea : IMessage
    {
        [Key(0)]
        public FixedVector3 Position { get; set; }

    }
}