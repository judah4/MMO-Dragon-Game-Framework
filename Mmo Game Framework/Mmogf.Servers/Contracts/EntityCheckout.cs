using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct EntityCheckout
    {

        [DataMember(Order = 0)]
        public List<EntityId> Checkouts { get; set; }
        [DataMember(Order = 1)]
        public bool Remove { get; set; }
    }
}
