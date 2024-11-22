using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct MmoMessage
    {
        [DataMember(Order = 1)]
        public ServerCodes MessageId { get; set; }
        [DataMember(Order = 2)]
        public byte[] Info { get; set; }
    }
}