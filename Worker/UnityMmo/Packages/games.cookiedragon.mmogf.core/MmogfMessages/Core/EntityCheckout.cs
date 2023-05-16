using MessagePack;
using System.Collections.Generic;

namespace Mmogf.Core
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
