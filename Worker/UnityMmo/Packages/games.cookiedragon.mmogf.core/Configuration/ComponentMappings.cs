using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{

    public static class ComponentMappings
    {

        static Dictionary<int, System.Type> _types = null;


        public static void Init(Dictionary<int, System.Type> types)
        {
            _types = types;
        }

        public static System.Type GetType(int componentId)
        {
            if(_types == null)
                return null;

            System.Type type;
            if(_types.TryGetValue(componentId, out type))
                return type;

            Debug.LogError($"Component {componentId} not mapped!");
            return null;
        }

    }
}
