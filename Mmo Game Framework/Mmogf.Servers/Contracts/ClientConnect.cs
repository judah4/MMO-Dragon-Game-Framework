using System.Runtime.Serialization;

namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct ClientConnect
    {
        [DataMember(Order = 0)]
        public long ClientId { get; set; }

    }
}