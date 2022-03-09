using MessagePack;
using Mmogf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf
{
    [MessagePackObject]
    public struct Cannon : IEntityComponent
    {
        public const int ComponentId = 1001;
        public int GetComponentId()
        {
            return ComponentId;
        }

        //[Key(0)]
        //public Position Position { get; set; }

        #region Events

        [MessagePackObject]
        public struct FireEvent : IEvent
        {
            public const int EventId = 20001;
            public int GetEventId() => EventId;

            [Key(0)]
            public bool Left { get; set; }
        }

        #endregion

        #region Commands

        [MessagePackObject]
        public struct FireCommand: ICommand
        {
            public const int CommandId = 10001;
            public int GetCommandId() => CommandId;

            [Key(0)]
            public bool Left { get; set; }
        }

        #endregion
    }
}
