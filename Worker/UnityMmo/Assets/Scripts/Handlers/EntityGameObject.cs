using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;

public class EntityGameObject : MonoBehaviour
{
    public int EntityId;
    public Dictionary<int, ByteString> Data = new Dictionary<int, ByteString>();

    public event Action OnEntityUpdate;

    public void EntityUpdated()
    {
        OnEntityUpdate?.Invoke();

    }

    
}

