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
        public FixedVector3 Position { get; set; }
        [Key(2)]
        public Rotation Rotation { get; set; }
        [Key(3)]
        public Dictionary<short, byte[]> Components { get; set; }

        [Key(4)]
        public List<Acl> Acls { get; set; }

        public CreateEntityRequest(string entityType, FixedVector3 position, Rotation rotation, Dictionary<short, byte[]> components, List<Acl> acls)
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
        public struct ChangeInterestAreaCommand : ICommandBase<NothingInternal, NothingInternal>
        {
            public const short CommandId = 98;
            public short GetCommandId() => CommandId;

            [Key(0)]
            public NothingInternal? Request { get; set; }
            [Key(1)]
            public NothingInternal? Response { get; set; }

        }

        [MessagePackObject]
        public struct PingCommand : ICommandBase<NothingInternal, NothingInternal>
        {
            public const short CommandId = 99;
            public short GetCommandId() => CommandId;

            [Key(0)]
            public NothingInternal? Request { get; set; }
            [Key(1)]
            public NothingInternal? Response { get; set; }

        }


        [MessagePackObject]
        public struct CreateEntity : ICommandBase<CreateEntityRequest, NothingInternal>
        {
            public const short CommandId = 100;
            public short GetCommandId() => CommandId;

            [Key(0)]
            public CreateEntityRequest? Request { get; set; }
            [Key(1)]
            public NothingInternal? Response { get; set; }

        }

        [MessagePackObject]
        public struct DeleteEntity : ICommandBase<DeleteEntityRequest, NothingInternal>
        {
            public const short CommandId = 101;
            public short GetCommandId() => CommandId;

            [Key(0)]
            public DeleteEntityRequest? Request { get; set; }
            [Key(1)]
            public NothingInternal? Response { get; set; }

        }

    }
}