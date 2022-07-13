using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    [MessagePackObject]
    public struct ConnectPlayerRequest
    {
        [Key(0)]
        public string PlayerId { get; set; }
    }

    [MessagePackObject]
    public struct PlayerCreator : IEntityComponent
    {

        public static int ComponentId = 7;
        public int GetComponentId() => ComponentId;

        [MessagePackObject]
        public struct ConnectPlayer : ICommandBase<ConnectPlayerRequest, NothingInternal>
        {
            public const int CommandId = 103;
            public int GetCommandId() => CommandId;

            [Key(0)]
            public ConnectPlayerRequest? Request { get; set; }

            [Key(1)]
            public NothingInternal? Response { get; set; }
        }

    }
}
