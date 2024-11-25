using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct MmoMessage : IMmoMessage
    {
        [DataMember(Order = 1)]
        public ServerCodes MessageId { get; set; }
        [DataMember(Order = 2)]
        public byte[] Info { get; set; }
    }

    public interface IMmoMessage
    {
        [DataMember(Order = 1)]
        ServerCodes MessageId { get; }
    }

    [DataContract]
    public struct MmoMessage<T> : IMmoMessage
    {
        [DataMember(Order = 1)]
        public ServerCodes MessageId { get; set; }
        [DataMember(Order = 2)]
        public T Info { get; set; }
    }
}