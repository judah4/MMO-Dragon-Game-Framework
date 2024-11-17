using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct ServerConnect
    {
        [DataMember(Order = 0)]
        public long ServerId { get; set; }
    }
}