using System.Runtime.Serialization;

namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct ConnectPlayerRequest
    {
        [DataMember(Order = 1)]
        public string PlayerId { get; set; }
    }

    [DataContract]
    public struct PlayerCreator : IEntityComponent
    {

        public const short ComponentId = 7;
        public short GetComponentId() => ComponentId;

        [DataContract]
        public struct ConnectPlayer : ICommandBase<ConnectPlayerRequest, NothingInternal>
        {
            public const short CommandId = 103;
            public short GetCommandId() => CommandId;

            [DataMember(Order = 1)]
            public ConnectPlayerRequest? Request { get; set; }

            [DataMember(Order = 2)]
            public NothingInternal? Response { get; set; }
        }
    }
}
