using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    [MessagePackObject]
    public partial struct EntityCheckout : IEntityComponent
    {
        public static int ComponentId = 8;
        public int GetComponentId() { return ComponentId; }

        [Key(0)]
        public List<long> Checkouts { get; set; }
    }
}
