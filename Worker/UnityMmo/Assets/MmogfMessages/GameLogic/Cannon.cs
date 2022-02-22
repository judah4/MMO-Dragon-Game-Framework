using MessagePack;
using Mmogf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragongf.Assets.MmogfMessages.GameLogic
{
    [MessagePackObject]
    public struct Cannon : IEntityComponent
    {
        public const int ComponentId = 10001;
        public int GetComponentId()
        {
            return ComponentId;
        }

        //[Key(0)]
        //public Position Position { get; set; }

        #region Events

        [MessagePackObject]
        public struct FireEvent : IMessage
        {
            [Key(0)]
            public bool Left { get; set; }
        }

        #endregion

        #region Commands

        [MessagePackObject]
        public struct FireCommand: ICommand
        {
            [Key(0)]
            public bool Left { get; set; }
        }

        #endregion
    }
}
