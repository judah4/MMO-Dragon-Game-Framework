using Mmogf.Servers.Shared;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct EntityCheckout
    {

        [DataMember(Order = 1)]
        public List<EntityId> Checkouts { get; set; }
        [DataMember(Order = 2)]
        public bool Remove { get; set; }
    }
}
