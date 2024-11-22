using System.Runtime.Serialization;

namespace Mmogf.Core.Contracts.Events
{
    [DataContract]
    public struct EventRequest
    {
        [DataMember(Order = 1)]
        public EventRequestHeader Header { get; set; }
        [DataMember(Order = 2)]
        public byte[] Payload { get; set; }
    }
}
