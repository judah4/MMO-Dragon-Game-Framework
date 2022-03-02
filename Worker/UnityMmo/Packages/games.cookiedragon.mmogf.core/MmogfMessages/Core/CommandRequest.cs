using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct CommandRequest : IMessage
    {
        [Key(0)]
        public string RequestId { get; set; }

        [Key(1)]
        public long RequesterId { get; set; }

        [Key(2)]
        public string RequestorWorkerType { get; set; }

        [Key(3)]
        public int EntityId { get; set; }
        [Key(4)]
        public int ComponentId { get; set; }
        [Key(5)]
        public string Command { get; set; }
        [Key(6)]
        public byte[] Payload { get; set; }
    }
}