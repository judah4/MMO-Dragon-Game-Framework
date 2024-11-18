using Mmogf.Servers.Shared;
using MessagePack;
namespace Mmogf.Core.Contracts.Events
{
    [MessagePackObject]
    public struct EventRequestHeader
    {
        [Key(0)]
        public EntityId EntityId { get; set; }
        [Key(1)]
        public short ComponentId { get; set; }
        [Key(2)]
        public short EventId { get; set; }

        public EventRequestHeader(EntityId entityId, short componentId, short eventId)
        {
            EntityId = entityId;
            ComponentId = componentId;
            EventId = eventId;
        }
    }
}