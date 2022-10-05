using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class PlayerMovementVisualizer : UpdatePositionReceiver
    {
        // Start is called before the first frame update
        void OnEnable()
        {
            var clientAuthCheck = GetEntityComponent<ClientAuthCheck>(ClientAuthCheck.ComponentId);
            var hasAuth = clientAuthCheck.HasValue && clientAuthCheck.Value.WorkerId == Server.ClientId;
            LocalControl = hasAuth;

            Entity.OnEntityUpdate += Entity_OnEntityUpdate;

        }

        private void Entity_OnEntityUpdate(int componentId)
        {
            var clientAuthCheck = GetEntityComponent<ClientAuthCheck>(ClientAuthCheck.ComponentId);
            var hasAuth = clientAuthCheck.HasValue && clientAuthCheck.Value.WorkerId == Server.ClientId;
            LocalControl = hasAuth;
        }
    }

}
