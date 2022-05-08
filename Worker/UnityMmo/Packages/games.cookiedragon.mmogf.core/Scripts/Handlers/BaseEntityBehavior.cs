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

       protected bool HasAuthority(int componentId)
        {
            var acls = GetEntityComponent<Acls>(Acls.ComponentId);

            if(acls.HasValue == false)
                return false;

            for(int cnt = 0; cnt <  acls.Value.AclList.Count; cnt++)
            {
                var acl = acls.Value.AclList[cnt];
                if(acl.ComponentId != componentId)
                    continue;

                return acl.WorkerType == Server.WorkerType && (acl.WorkerId == 0 || acl.WorkerId == Server.ClientId);
            }

            return false;
        }

    }
}
