using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct MmoMessageHeader
    {
        [DataMember(Order = 0)]
        public ServerCodes MessageId { get; set; }
    }
}