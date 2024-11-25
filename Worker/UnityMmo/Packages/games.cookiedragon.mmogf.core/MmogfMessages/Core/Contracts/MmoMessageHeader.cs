using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct MmoMessageHeader
    {
        [DataMember(Order = 1)]
        public ServerCodes MessageId { get; set; }
    }

    [DataContract]
    public struct MmoMessagePayload<T>
    {
        [DataMember(Order = 2)]
        public T Info { get; set; }
    }
}