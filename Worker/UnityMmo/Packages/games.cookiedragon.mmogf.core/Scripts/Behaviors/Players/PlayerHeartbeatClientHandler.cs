using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    public class PlayerHeartbeatClientHandler : BaseEntityBehavior
    {
        private void Update()
        {
            var hasAuth = HasAuthority(PlayerHeartbeatClient.ComponentId);
            if (!hasAuth)
                return;

            for (int cnt = 0; cnt < Server.CommandRequests.Count; cnt++)
            {
                if (Server.CommandRequests[cnt].ComponentId != PlayerHeartbeatClient.ComponentId)
                    continue;
                if (Server.CommandRequests[cnt].EntityId != Entity.EntityId)
                    continue;
                var request = Server.CommandRequests[cnt];
                //we need a way to identify what command this is... Components will be able to have more commands
                //use ids!
                switch (request.CommandId)
                {
                    case PlayerHeartbeatClient.RequestHeartbeat.CommandId:
                        HandleHeartbeat(request);
                        break;
                }

            }
        }

        void HandleHeartbeat(CommandRequest request)
        {
            Debug.Log("Got client heartbeat!");

            //make empty response object
            Server.SendCommandResponse(request, new PlayerHeartbeatClient.RequestHeartbeat());
        }
    }
}
