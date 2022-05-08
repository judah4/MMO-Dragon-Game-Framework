using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mmogf.Core
{
    public class BaseEntityBehavior : MonoBehaviour
    {
        public CommonHandler Server { get; set; }
        public EntityGameObject Entity { get; set; }

        protected T? GetEntityComponent<T>(int componentId) where T : struct, IEntityComponent
        {
            IEntityComponent component;
            if (Entity.Data.TryGetValue(componentId, out component))
            {
                return (T)component;
            }

            return null;

        }

    }
}
