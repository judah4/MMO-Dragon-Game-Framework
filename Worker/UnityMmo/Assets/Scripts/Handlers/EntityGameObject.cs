using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EntityGameObject : MonoBehaviour
{
    public int EntityId;
    public Dictionary<int, byte[]> Data = new Dictionary<int, byte[]>();

    public event Action OnEntityUpdate;

    public void EntityUpdated()
    {
        OnEntityUpdate?.Invoke();

    }

    
}

