using MessagePack;
using Mmogf.Core;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct ClientConnect
    {
        [Key(0)]
        public long ClientId { get; set; }

    }
}