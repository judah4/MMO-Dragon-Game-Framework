using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    [MessagePackObject]
    public partial struct Acls : IEntityComponent
    {
        public static short ComponentId = 4;
        public short GetComponentId() { return ComponentId; }

        [Key(0)]
        public List<Acl> AclList { get; set; }
    }
}
