using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct SimpleMessage : IMessage
    {
        [Key(0)]
        public int MessageId { get; set; }
        [Key(1)]
        public byte[] Info { get; set; }
    }
}