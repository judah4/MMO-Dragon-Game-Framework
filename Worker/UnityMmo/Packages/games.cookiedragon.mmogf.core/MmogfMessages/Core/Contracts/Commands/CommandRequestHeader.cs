using MessagePack;
using Mmogf.Servers.Shared;
namespace Mmogf.Core.Contracts.Commands
{
    [MessagePackObject]
    public struct CommandRequestHeader
    {
        [Key(0)]
        public string RequestId { get; set; }

        [Key(1)]
        public long RequesterId { get; set; }

        [Key(2)]
        public string RequestorWorkerType { get; set; }

        [Key(3)]
        public EntityId EntityId { get; set; }
        [Key(4)]
        public short ComponentId { get; set; }
        [Key(5)]
        public short CommandId { get; set; }
    }
}