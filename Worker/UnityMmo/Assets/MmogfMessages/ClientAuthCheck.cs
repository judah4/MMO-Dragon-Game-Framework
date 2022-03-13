using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    [MessagePackObject]
    public struct ClientAuthCheck : IEntityComponent
    {
        public const int ComponentId = 1003;
        public int GetComponentId()
        {
            return ComponentId;
        }

        [Key(0)]
        public long WorkerId { get; set; }
    }
}
