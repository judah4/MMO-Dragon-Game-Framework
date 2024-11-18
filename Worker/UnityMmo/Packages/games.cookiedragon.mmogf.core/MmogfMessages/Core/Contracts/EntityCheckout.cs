using Mmogf.Servers.Shared;
using System.Collections.Generic;
using MessagePack;

namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct EntityCheckout
    {

        [Key(0)]
        public List<EntityId> Checkouts { get; set; }
        [Key(1)]
        public bool Remove { get; set; }
    }
}
