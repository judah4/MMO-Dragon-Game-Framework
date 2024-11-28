using Mmogf.Core.Contracts;
using System.Runtime.Serialization;

namespace Mmogf
{
    [DataContract]
    public struct ClientAuthCheck : IEntityComponent
    {
        public const short ComponentId = 1003;
        public short GetComponentId()
        {
            return ComponentId;
        }

        [DataMember(Order = 1)]
        public long WorkerId { get; set; }
    }
}
