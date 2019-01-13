using System.Collections;
using System.Collections.Generic;
using MessageProtocols.Server;
using Mmogf.Core;
using UnityEngine;

public class GameObjectRepresentation
{
    private Dictionary<int, EntityGameObject> _entities = new Dictionary<int, EntityGameObject>();

    public void OnEntityCreation(EntityInfo entity)
    {
        EntityGameObject entityGm;

        var entityType = EntityType.Parser.ParseFrom(entity.EntityData[1]);
        var position = Position.Parser.ParseFrom(entity.EntityData[2]);
        var adjustedPos = new Vector3((int) position.X, (int) position.Y, (int) position.Z);

        if (!_entities.TryGetValue(entity.EntityId, out entityGm))
        {
            var prefab = Resources.Load<GameObject>(entityType.Name);
            var gm = Object.Instantiate(prefab, adjustedPos,
                Quaternion.identity);
            gm.name = $"{entityType.Name} : {entity.EntityId}";

            entityGm = gm.AddComponent<EntityGameObject>();
            entityGm.EntityId = entity.EntityId;
            _entities.Add(entity.EntityId, entityGm);
        }
        else
        {
            entityGm.transform.position = adjustedPos;
        }

        //update the datas


    }
}
