using MessagePack;
using Mmogf.Core;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct ChangeInterestArea : IMessage
    {
        [Key(0)]
        public Position Position { get; set; }

    }
}