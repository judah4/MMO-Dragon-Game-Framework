using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct MmoMessageHeader
    {
        [DataMember(Order = 1)]
        public ServerCodes MessageId { get; set; }
    }
}