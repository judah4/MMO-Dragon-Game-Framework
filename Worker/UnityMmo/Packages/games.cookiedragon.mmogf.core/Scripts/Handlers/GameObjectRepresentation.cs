using MessagePack;
using MessagePack.Resolvers;
using Mmogf;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameObjectRepresentation
{

    public Dictionary<int, EntityGameObject> Entities => _entities;

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
        var rot = MessagePackSerializer.Deserialize<Rotation>(entity.EntityData[Rotation.ComponentId]);
        var adjustedPos = _server.PositionToClient(position);
        var rotation = rot.ToQuaternion();

        if (!_entities.TryGetValue(entity.EntityId, out entityGm))
        {
            var prefab = Resources.Load<GameObject>($"Prefabs/{_server.WorkerType}/{entityType.Name}");

            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>($"Prefabs/common/{entityType.Name}");
            }

            var gm = Object.Instantiate(prefab, adjustedPos,
                rotation);
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
            var type = ComponentMappings.GetType(pair.Key);
            
            entityGm.Data.Add(pair.Key, (IEntityComponent)MessagePackSerializer.Deserialize(type, pair.Value));
        }
    }

    public void OnEntityUpdate(EntityUpdate entityUpdate)
    {
        EntityGameObject entityGm;

        if (!_entities.TryGetValue(entityUpdate.EntityId, out entityGm))
            return;

        entityGm.Data.Remove(entityUpdate.ComponentId);
        var type = ComponentMappings.GetType(entityUpdate.ComponentId);

        var data = (IEntityComponent)MessagePackSerializer.Deserialize(type, entityUpdate.Info);
        entityGm.Data.Add(entityUpdate.ComponentId, data);

        entityGm.EntityUpdated();
    }

    public void OnEntityDelete(EntityInfo entity)
    {

        //var entityType = MessagePackSerializer.Deserialize<EntityType>(entity.EntityData[EntityType.ComponentId]);
        //var position = MessagePackSerializer.Deserialize<Position>(entity.EntityData[Position.ComponentId]);
        //var rot = MessagePackSerializer.Deserialize<Rotation>(entity.EntityData[Rotation.ComponentId]);
        //var adjustedPos = _server.PositionToClient(position);
        //var rotation = rot.ToQuaternion();

        DeleteEntity(entity.EntityId);
    }

    public void DeleteEntity(int entityId)
    {
        EntityGameObject entityGm;

        if (!_entities.TryGetValue(entityId, out entityGm))
            return;

        Object.Destroy(entityGm.gameObject);
        _entities.Remove(entityId);
    }

    public void UpdateEntity<T>(int entityId, int componentId, T message) where T : IEntityComponent
    {
        EntityGameObject entityGm;
        if (!_entities.TryGetValue(entityId, out entityGm))
            return;

        entityGm.Data.Remove(componentId);
        entityGm.Data.Add(componentId, message);

        entityGm.EntityUpdated();

    }
}
