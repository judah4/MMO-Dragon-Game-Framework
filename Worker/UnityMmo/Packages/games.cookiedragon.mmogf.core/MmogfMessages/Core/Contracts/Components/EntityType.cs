using MessagePack;

namespace Mmogf.Core.Contracts
{

    [MessagePackObject]
    public struct EntityType : IEntityComponent
    {
        public const short ComponentId = 1;
        public short GetComponentId() => ComponentId;

        [Key(0)]
        public string Name { get; set; }
    }
}

