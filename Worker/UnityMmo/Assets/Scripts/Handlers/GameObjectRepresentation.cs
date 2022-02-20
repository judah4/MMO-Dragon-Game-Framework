using MessagePack;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameObjectRepresentation
{
    private Dictionary<int, EntityGameObject> _entities = new Dictionary<int, EntityGameObject>();

    private CommonHandler _server;

    public GameObjectRepresentation(CommonHandler commonHandler)
    {
        _server = commonHandler;
    }

    public void OnEntityCreation(EntityInfo entity)
    {
        EntityGameObject entityGm;

        var entityType = MessagePackSerializer.Deserialize<EntityType>(entity.EntityData[EntityType.ComponentId]);
        var position = MessagePackSerializer.Deserialize<Position>(entity.EntityData[Position.ComponentId]);
        var adjustedPos = _server.PositionToClient(position);

        if (!_entities.TryGetValue(entity.EntityId, out entityGm))
        {
            var prefab = Resources.Load<GameObject>($"Prefabs/{_server.WorkerType}/{entityType.Name}");

            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>($"Prefabs/common/{entityType.Name}");
            }

            var gm = Object.Instantiate(prefab, adjustedPos,
                Quaternion.identity);
            gm.name = $"{entityType.Name} : {entity.EntityId} - {_server.WorkerType}";

            entityGm = gm.AddComponent<EntityGameObject>();

            var entityBehaviors = entityGm.GetComponents<BaseEntityBehavior>();
            for (int cnt = 0; cnt < entityBehaviors.Length; cnt++)
            {
                entityBehaviors[cnt].Server = _server;
                entityBehaviors[cnt].Entity = entityGm;
                entityBehaviors[cnt].enabled = true;
            }

            entityGm.EntityId = entity.EntityId;
            _entities.Add(entity.EntityId, entityGm);
        }

        foreach (var pair in entity.EntityData)
        {
            entityGm.Data.Remove(pair.Key);
            entityGm.Data.Add(pair.Key, pair.Value);
        }


    }

    public void OnEntityUpdate(EntityUpdate entityUpdate)
    {
        EntityGameObject entityGm;

        Debug.Log(entityUpdate.ToString());

        if (!_entities.TryGetValue(entityUpdate.EntityId, out entityGm))
            return;

        entityGm.Data.Remove(entityUpdate.ComponentId);
        entityGm.Data.Add(entityUpdate.ComponentId, entityUpdate.Info);

        entityGm.EntityUpdated();
    }

    public void UpdateEntity(int entityId, int componentId, object message)
    {
        EntityGameObject entityGm;
        if (!_entities.TryGetValue(entityId, out entityGm))
            return;

        entityGm.Data.Remove(componentId);
        entityGm.Data.Add(componentId, MessagePackSerializer.Serialize(message));

        entityGm.EntityUpdated();

    }
}
