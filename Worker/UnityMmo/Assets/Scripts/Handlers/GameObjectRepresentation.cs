using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using MessageProtocols;
using MessageProtocols.Core;
using MessageProtocols.Server;
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

        var entityType = EntityType.Parser.ParseFrom(entity.EntityData[1]);
        var position = Position.Parser.ParseFrom(entity.EntityData[2]);
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

    public void UpdateEntity(int entityId, IEntityComponent entityComponent)
    {
        EntityGameObject entityGm;
        if (!_entities.TryGetValue(entityId, out entityGm))
            return;

        var message = entityComponent as IMessage;

        entityGm.Data.Remove(entityComponent.GetComponentId());
        entityGm.Data.Add(entityComponent.GetComponentId(), message.ToByteString());

        entityGm.EntityUpdated();

    }
}
