using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    [MessagePackObject]
    public struct PlayerCreator : IEntityComponent
    {

        public static int ComponentId = 7;
        public int GetComponentId() => ComponentId;

        [MessagePackObject]
        public struct ConnectPlayer : ICommand
        {
            public const int CommandId = 103;
            public int GetCommandId() => CommandId;

            [Key(0)]
            public string PlayerId { get; set; }


        }

    }
}
