using MessagePack;
using System.Collections.Generic;

namespace Mmogf.Core
{
    /// <summary>
    /// Access Control List. Component for tracking data ownership for clients and server workers.
    /// </summary>
    [MessagePackObject]
    public partial struct Acls : IEntityComponent
    {
        public const short ComponentId = 4;
        public short GetComponentId() { return ComponentId; }

        [Key(0)]
        public List<Acl> AclList { get; set; }
    }
}
