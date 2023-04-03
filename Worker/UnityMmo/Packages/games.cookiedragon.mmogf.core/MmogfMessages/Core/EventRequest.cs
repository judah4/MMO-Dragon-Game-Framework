using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct EventRequest : IMessage
    {
        [Key(0)]
        public EntityId EntityId { get; set; }
        [Key(1)]
        public short ComponentId { get; set; }
        [Key(2)]
        public short EventId { get; set; }
        [Key(3)]
        public byte[] Payload { get; set; }
    }
}