using System.Collections;
using System.Collections.Generic;
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
        var adjustedPos = new Vector3((int) position.X, (int) position.Y, (int) position.Z) + _server.transform.position;

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
