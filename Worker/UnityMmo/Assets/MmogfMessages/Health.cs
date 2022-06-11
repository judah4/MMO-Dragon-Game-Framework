using MessagePack;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{

    [MessagePackObject]
    public struct TakeDamageResponse
    {
        [Key(0)]
        public bool Dead { get; set; }
    }

    [MessagePackObject]
    public struct TakeDamageCommandRequest
    {
        [Key(0)]
        public int Amount { get; set; }
    }

    [MessagePackObject]
    public struct Health : IEntityComponent
    {
        public const int ComponentId = 1002;
        public int GetComponentId()
        {
            return ComponentId;
        }

        [Key(0)]
        public int Current { get; set; }
        [Key(1)]
        public int Max { get; set; }

        #region Commands

        [MessagePackObject]
        public struct TakeDamageCommand : ICommandBase<TakeDamageCommandRequest, TakeDamageResponse>
        {
            public const int CommandId = 10002;
            public int GetCommandId() => CommandId;

            [Key(0)]
            public TakeDamageCommandRequest? Request { get; set; }
            [Key(1)]
            public TakeDamageResponse? Response { get; set; }

        }

        #endregion

    }
}
