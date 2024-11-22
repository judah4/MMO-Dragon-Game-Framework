using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct GameData
    {
        [DataMember(Order = 1)]
        public int EntityId { get; set; }
        [DataMember(Order = 2)]
        public byte[] Info { get; set; }

    }
}