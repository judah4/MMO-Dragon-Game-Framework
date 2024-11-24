using MmoGameFramework;
using Mmogf.Core.Contracts;
using Mmogf.Servers.Shared;
using System.Collections.Generic;

namespace Mmogf.Servers.ServerInterfaces
{
    public interface IEntityStore
    {
        ImmutableEntity CreateEntity(string entityType, Position position, Rotation rotation, List<Acl> acls);

        ImmutableEntity Get(EntityId entityId);

        void Update(Entity entity);

        ImmutableEntity Update(EntityId entityId, short componentId, IComponentData data);

        ImmutableEntity Delete(EntityId entityId);
    }
}
