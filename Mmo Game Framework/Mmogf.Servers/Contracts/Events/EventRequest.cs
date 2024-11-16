using System.Runtime.Serialization;

namespace Mmogf.Servers.Contracts.Events
{
    [DataContract]
    public struct EventRequest
    {
        [DataMember(Order = 0)]
        public EventRequestHeader Header { get; set; }
        [DataMember(Order = 1)]
        public byte[] Payload { get; set; }
    }
}
