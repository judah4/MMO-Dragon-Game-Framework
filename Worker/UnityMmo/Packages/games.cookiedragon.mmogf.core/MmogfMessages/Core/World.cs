using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct CreateEntityRequest
    {
        [Key(0)]
        public string EntityType { get; set; }
        [Key(1)]
        public Position Position { get; set; }
        [Key(2)]
        public Rotation Rotation { get; set; }
        [Key(3)]
        public Dictionary<int, byte[]> Components { get; set; }

        [Key(4)]
        public List<Acl> Acls { get; set; }

        public CreateEntityRequest(string entityType, Position position, Rotation rotation, Dictionary<int, byte[]> components, List<Acl> acls)
        {
            EntityType = entityType;
            Position = position;
            Rotation = rotation;
            Components = components;
            Acls = acls;
        }

    }

    [MessagePackObject]
    public struct DeleteEntityRequest
    {
        [Key(0)]
        public int EntityId { get; set; }

        public DeleteEntityRequest(int entityId)
        {
            EntityId = entityId;
        }
    }

    public struct World
    {

        [MessagePackObject]
        public struct CreateEntity : ICommandBase<CreateEntityRequest, NothingInternal>
        {
            public const int CommandId = 100;
            public int GetCommandId() => CommandId;

            [Key(0)]
            public CreateEntityRequest? Request { get; set; }
            [Key(1)]
            public NothingInternal? Response { get; set; }

        }

        [MessagePackObject]
        public struct DeleteEntity : ICommandBase<DeleteEntityRequest, NothingInternal>
        {
            public const int CommandId = 101;
            public int GetCommandId() => CommandId;

            [Key(0)]
            public DeleteEntityRequest? Request { get; set; }
            [Key(1)]
            public NothingInternal? Response { get; set; }

        }

    }
}