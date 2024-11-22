using Mmogf.Servers.Shared;
using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts.Commands
{
    [DataContract]
    public struct CommandRequestHeader
    {
        [DataMember(Order = 1)]
        public string RequestId { get; set; }

        [DataMember(Order = 2)]
        public long RequesterId { get; set; }

        [DataMember(Order = 3)]
        public string RequestorWorkerType { get; set; }

        [DataMember(Order = 4)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 5)]
        public short ComponentId { get; set; }
        [DataMember(Order = 6)]
        public short CommandId { get; set; }
    }
}