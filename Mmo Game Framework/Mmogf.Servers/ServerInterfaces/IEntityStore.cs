using MmoGameFramework;
using Mmogf.Core.Contracts;
using Mmogf.Servers.Shared;
using System.Collections.Generic;

namespace Mmogf.Servers.ServerInterfaces
{
    public interface IEntityStore
    {
        Entity CreateEntity(string entityType, Position position, Rotation rotation, List<Acl> acls);

        Entity Get(EntityId entityId);

        void Update(Entity entity);

        Entity Update(EntityId entityId, short componentId, IComponentData data);

        Entity Delete(EntityId entityId);
    }
}
