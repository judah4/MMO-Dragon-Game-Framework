using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct WorldConfig
    {
        [DataMember(Order = 1)]
        public string Version { get; set; }

        [DataMember(Order = 2)]
        public List<EntityWorldConfig> Entities { get; set; }


    }
}