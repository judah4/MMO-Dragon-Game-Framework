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

        public static short ComponentId = 7;
        public short GetComponentId() => ComponentId;

        [MessagePackObject]
        public struct ConnectPlayer : ICommandBase<ConnectPlayerRequest, NothingInternal>
        {
            public const short CommandId = 103;
            public short GetCommandId() => CommandId;

            [Key(0)]
            public ConnectPlayerRequest? Request { get; set; }

            [Key(1)]
            public NothingInternal? Response { get; set; }
        }

    }
}
