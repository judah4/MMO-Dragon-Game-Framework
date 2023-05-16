using Mmogf.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityGameObject : MonoBehaviour
{
    public EntityId EntityId;
    public Dictionary<int, IEntityComponent> Data = new Dictionary<int, IEntityComponent>();

    public event Action<int> OnEntityUpdate;
    public List<int> Updates = new List<int>(10); //cleared in a batch on GameObjectRepresentation

    public CommonHandler Server { get; set; }


    public T? GetEntityComponent<T>(int componentId) where T : struct, IEntityComponent
    {
        IEntityComponent component;
        if (Data.TryGetValue(componentId, out component))
        {
            return (T)component;
        }

        return null;

    }

    public bool HasAuthority(int componentId)
    {
        var acls = GetEntityComponent<Acls>(Acls.ComponentId);

        if (acls.HasValue == false)
            return false;

        for (int cnt = 0; cnt < acls.Value.AclList.Count; cnt++)
        {
            var acl = acls.Value.AclList[cnt];
            if (acl.ComponentId != componentId)
                continue;

            return acl.WorkerType == Server.WorkerType && (acl.WorkerId == 0 || acl.WorkerId == Server.ClientId);
        }

        return false;
    }

    public void EntityUpdated(int componentId)
    {
        Updates.Add(componentId);
        OnEntityUpdate?.Invoke(componentId);

    }
 
}

