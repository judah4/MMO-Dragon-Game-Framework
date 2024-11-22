using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct ServerConnect
    {
        [DataMember(Order = 1)]
        public long ServerId { get; set; }
    }
}