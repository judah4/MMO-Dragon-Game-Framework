using Mmogf.Core.Contracts;
using System.Runtime.Serialization;

namespace Mmogf
{

    [DataContract]
    public struct TakeDamageResponse
    {
        [DataMember(Order = 1)]
        public bool Dead { get; set; }
        [DataMember(Order = 2)]
        public bool Killed { get; set; }
    }

    [DataContract]
    public struct TakeDamageRequest
    {
        [DataMember(Order = 1)]
        public int Amount { get; set; }
    }

    [DataContract]
    public struct Health : IEntityComponent
    {
        public const short ComponentId = 1002;
        public short GetComponentId()
        {
            return ComponentId;
        }

        [DataMember(Order = 1)]
        public int Current { get; set; }
        [DataMember(Order = 2)]
        public int Max { get; set; }

        #region Commands

        [DataContract]
        public struct TakeDamageCommand : ICommandBase<TakeDamageRequest, TakeDamageResponse>
        {
            public const short CommandId = 10002;
            public short GetCommandId() => CommandId;

            [DataMember(Order = 1)]
            public TakeDamageRequest? Request { get; set; }
            [DataMember(Order = 2)]
            public TakeDamageResponse? Response { get; set; }

        }

        #endregion

    }
}
