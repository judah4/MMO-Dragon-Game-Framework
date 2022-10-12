using MessagePack;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    [MessagePackObject]
    public struct ClientAuthCheck : IEntityComponent
    {
        public const short ComponentId = 1003;
        public short GetComponentId()
        {
            return ComponentId;
        }

        [Key(0)]
        public long WorkerId { get; set; }
    }
}
