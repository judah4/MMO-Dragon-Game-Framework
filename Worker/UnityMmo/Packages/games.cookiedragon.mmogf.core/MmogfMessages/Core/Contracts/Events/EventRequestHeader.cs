using Mmogf.Servers.Shared;
using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts.Events
{
    [DataContract]
    public struct EventRequestHeader
    {
        [DataMember(Order = 0)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 1)]
        public short ComponentId { get; set; }
        [DataMember(Order = 2)]
        public short EventId { get; set; }

        public EventRequestHeader(EntityId entityId, short componentId, short eventId)
        {
            EntityId = entityId;
            ComponentId = componentId;
            EventId = eventId;
        }
    }
}