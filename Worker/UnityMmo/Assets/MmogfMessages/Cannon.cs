using MessagePack;
using Mmogf;
using Mmogf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf
{

    [MessagePackObject]
    public struct FireCommandRequest
    {
        [Key(0)]
        public bool Left { get; set; }
    }

    [MessagePackObject]
    public struct Cannon : IEntityComponent
    {
        public const short ComponentId = 1001;
        public short GetComponentId()
        {
            return ComponentId;
        }

        //[Key(0)]
        //public Position Position { get; set; }

        #region Events

        [MessagePackObject]
        public struct FireEvent : IEvent
        {
            public const short EventId = 20001;
            public short GetEventId() => EventId;

            [Key(0)]
            public bool Left { get; set; }
        }

        #endregion

        #region Commands

        [MessagePackObject]
        public struct FireCommand : ICommandBase<FireCommandRequest, Nothing>
        {
            public const short CommandId = 10001;
            public short GetCommandId() => CommandId;

            [Key(0)]
            public FireCommandRequest? Request { get; set; }
            [Key(1)]
            public Nothing? Response { get; set; }

        }

        #endregion
    }
}
