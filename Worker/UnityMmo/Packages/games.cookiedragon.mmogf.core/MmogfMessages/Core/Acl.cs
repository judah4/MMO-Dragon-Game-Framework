using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;

namespace Mmogf.Core
{
    public enum AclAccess
    {
        None,
        Read,
        Write,
    }

    /// <summary>
    /// Access Control List
    /// </summary>
    [MessagePackObject]
    public struct Acl : IMessage
    {
        [Key(0)]
        public short ComponentId { get; set; }

        [Key(1)]
        public string WorkerType { get; set; }

        [Key(2)]
        public long WorkerId { get; set; }

    }

}