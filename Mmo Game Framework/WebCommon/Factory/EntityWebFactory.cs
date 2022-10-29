using MmoGameFramework;
using MmoGameFramework.Models;

namespace WebCommon.Factory;

public static class EntityWebFactory
{

    public static EntityModel Convert(Entity entity)
    {
        var model = new EntityModel
        {
            Id = entity.EntityId,
            Position = entity.Position
        };

        return model;
    }
}