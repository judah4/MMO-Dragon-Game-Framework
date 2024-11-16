using System.Runtime.Serialization;
namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct MmoMessageHeader
    {
        [DataMember(Order = 0)]
        public ServerCodes MessageId { get; set; }
    }
}