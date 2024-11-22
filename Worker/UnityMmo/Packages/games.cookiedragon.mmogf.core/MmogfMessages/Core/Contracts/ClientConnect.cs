using System.Runtime.Serialization;

namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct ClientConnect
    {
        [DataMember(Order = 1)]
        public long ClientId { get; set; }

    }
}