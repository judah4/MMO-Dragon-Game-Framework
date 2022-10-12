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

    List<int> _cleanEntityIds = new List<int>();

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

        BaseEntityBehavior[] entityBehaviors = null;

        if (!_entities.TryGetValue(entity.EntityId, out entityGm))
        {
            var prefab = Resources.Load<GameObject>($"Prefabs/{_server.WorkerType}/{entityType.Name}");

            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>($"Prefabs/common/{entityType.Name}");
            }
            if(prefab == null)
                return;

            var gm = Object.Instantiate(prefab, adjustedPos,
                rotation);
            gm.name = $"{entityType.Name} : {entity.EntityId} - {_server.WorkerType}";

            entityGm = gm.AddComponent<EntityGameObject>();
            entityGm.Server = _server;
            entityGm.EntityId = entity.EntityId;

            entityBehaviors = entityGm.GetComponents<BaseEntityBehavior>();
            for (int cnt = 0; cnt < entityBehaviors.Length; cnt++)
            {
                entityBehaviors[cnt].Server = _server;
                entityBehaviors[cnt].Entity = entityGm;
            }

            _entities.Add(entity.EntityId, entityGm);
        }

        foreach (var pair in entity.EntityData)
        {
            entityGm.Data.Remove(pair.Key);
            var type = ComponentMappings.GetComponentType(pair.Key);

            if(type == null)
            {
                Debug.LogWarning($"Type mapping not found! ComponentId:{pair.Key}");
                continue;
            }
            
            try
            {
                entityGm.Data.Add(pair.Key, (IEntityComponent)MessagePackSerializer.Deserialize(type, pair.Value));
            }
            catch(System.Exception e)
            {
                Debug.LogError($"Error deserializing during create! Type:{type}");
                Debug.LogException(e);
            }
        }

        if(entityBehaviors != null) 
        {
            for (int cnt = 0; cnt < entityBehaviors.Length; cnt++)
            {
                entityBehaviors[cnt].enabled = true;
            }
        }
    }

    public void OnEntityUpdate(EntityUpdate entityUpdate)
    {
        EntityGameObject entityGm;

        if (!_entities.TryGetValue(entityUpdate.EntityId, out entityGm))
            return;

        entityGm.Data.Remove(entityUpdate.ComponentId);
        var type = ComponentMappings.GetComponentType(entityUpdate.ComponentId);

        var data = (IEntityComponent)MessagePackSerializer.Deserialize(type, entityUpdate.Info);
        entityGm.Data.Add(entityUpdate.ComponentId, data);

        entityGm.EntityUpdated(entityUpdate.ComponentId);

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

        entityGm.EntityUpdated(componentId);

    }

    public void Update()
    {
        foreach (var entity in _entities)
        {
            entity.Value.Updates.Clear();

        }



        //        _cleanEntityIds.Clear();
        //        foreach (var entity in _entities)
        //        {
        //            if(entity.Value.HasAuthority(Position.ComponentId))
        //                continue;

        //            var dif = entity.Value.transform.position - _server.InterestCenter;
        //            if(dif.sqrMagnitude > 149f * 149f)
        //            {
        //                _cleanEntityIds.Add(entity.Key);
        //            }

        //        }

        //        for(int cnt = 0; cnt < _cleanEntityIds.Count; cnt++)
        //        {
        //#if UNITY_EDITOR
        //            Debug.Log($"Deleting {_cleanEntityIds[cnt]} from out of range.");
        //#endif
        //            DeleteEntity(_cleanEntityIds[cnt]);
        //        }
    }
}
