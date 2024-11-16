using System.Runtime.Serialization;
namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct GameData
    {
        [DataMember(Order = 0)]
        public int EntityId { get; set; }
        [DataMember(Order = 1)]
        public byte[] Info { get; set; }

    }
}