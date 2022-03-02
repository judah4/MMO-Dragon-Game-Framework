using Mmogf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EntityGameObject : MonoBehaviour
{
    public int EntityId;
    public Dictionary<int, IEntityComponent> Data = new Dictionary<int, IEntityComponent>();

    public event Action OnEntityUpdate;

    public void EntityUpdated()
    {
        OnEntityUpdate?.Invoke();

    }
 
}

