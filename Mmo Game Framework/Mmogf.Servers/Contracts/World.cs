using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct CreateEntityRequest
    {
        [DataMember(Order = 0)]
        public string EntityType { get; set; }
        [DataMember(Order = 1)]
        public FixedVector3 Position { get; set; }
        [DataMember(Order = 2)]
        public Rotation Rotation { get; set; }
        [DataMember(Order = 3)]
        public Dictionary<short, byte[]> Components { get; set; }

        [DataMember(Order = 4)]
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

    [DataContract]
    public struct DeleteEntityRequest
    {
        [DataMember(Order = 0)]
        public EntityId EntityId { get; set; }

        public DeleteEntityRequest(EntityId entityId)
        {
            EntityId = entityId;
        }
    }

    public interface IWorldCommand { }

    public struct World
    {

        [DataContract]
        public struct ChangeInterestAreaCommand : ICommandBase<NothingInternal, NothingInternal>, IWorldCommand
        {
            public const short CommandId = 98;
            public short GetCommandId() => CommandId;

            [DataMember(Order = 0)]
            public NothingInternal? Request { get; set; }
            [DataMember(Order = 1)]
            public NothingInternal? Response { get; set; }

        }

        [DataContract]
        public struct PingCommand : ICommandBase<NothingInternal, NothingInternal>, IWorldCommand
        {
            public const short CommandId = 99;
            public short GetCommandId() => CommandId;

            [DataMember(Order = 0)]
            public NothingInternal? Request { get; set; }
            [DataMember(Order = 1)]
            public NothingInternal? Response { get; set; }

        }


        [DataContract]
        public struct CreateEntity : ICommandBase<CreateEntityRequest, NothingInternal>, IWorldCommand
        {
            public const short CommandId = 100;
            public short GetCommandId() => CommandId;

            [DataMember(Order = 0)]
            public CreateEntityRequest? Request { get; set; }
            [DataMember(Order = 1)]
            public NothingInternal? Response { get; set; }

        }

        [DataContract]
        public struct DeleteEntity : ICommandBase<DeleteEntityRequest, NothingInternal>, IWorldCommand
        {
            public const short CommandId = 101;
            public short GetCommandId() => CommandId;

            [DataMember(Order = 0)]
            public DeleteEntityRequest? Request { get; set; }
            [DataMember(Order = 1)]
            public NothingInternal? Response { get; set; }

        }

    }
}