using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct MmoMessage
    {
        [DataMember(Order = 0)]
        public ServerCodes MessageId { get; set; }
        [DataMember(Order = 1)]
        public byte[] Info { get; set; }
    }
}