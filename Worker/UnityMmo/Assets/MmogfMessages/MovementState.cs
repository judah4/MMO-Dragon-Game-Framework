using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    [MessagePackObject]
    public struct MovementState : IEntityComponent
    {
        public const int ComponentId = 1004;
        public int GetComponentId()
        {
            return ComponentId;
        }

        [Key(0)]
        public float Forward { get; set; }
        [Key(1)]
        public float Heading { get; set; }
    }
}
