using Mmogf.Core.Contracts;
using System.Runtime.Serialization;

namespace Mmogf
{

    [DataContract]
    public struct FireCommandRequest
    {
        [DataMember(Order = 1)]
        public bool Left { get; set; }
    }

    [DataContract]
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

        [DataContract]
        public struct FireEvent : IEvent
        {
            public const short EventId = 20001;
            public short GetEventId() => EventId;

            [DataMember(Order = 1)]
            public bool Left { get; set; }
        }

        #endregion

        #region Commands

        [DataContract]
        public struct FireCommand : ICommandBase<FireCommandRequest, Nothing>
        {
            public const short CommandId = 10001;
            public short GetCommandId() => CommandId;

            [DataMember(Order = 1)]
            public FireCommandRequest? Request { get; set; }

            [DataMember(Order = 2)]
            public Nothing? Response { get; set; }

        }

        #endregion
    }
}
