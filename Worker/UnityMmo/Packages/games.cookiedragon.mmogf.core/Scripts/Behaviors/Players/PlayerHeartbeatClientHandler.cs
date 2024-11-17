using MessagePack;
using Mmogf.Core.Contracts;
using Mmogf.Core.Contracts.Commands;
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

            Server.UpdateInterestArea(transform.position);

            for (int cnt = 0; cnt < Server.CommandRequests.Count; cnt++)
            {
                var request = Server.CommandRequests[cnt];

                if (request.Header.ComponentId != PlayerHeartbeatClient.ComponentId)
                    continue;
                if (request.Header.EntityId != Entity.EntityId)
                    continue;
                //we need a way to identify what command this is... Components will be able to have more commands
                //use ids!
                switch (request.Header.CommandId)
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
            var payload = MessagePackSerializer.Deserialize<PlayerHeartbeatClient.RequestHeartbeat>(request.Payload);

            //make empty response object
            Server.SendCommandResponse<PlayerHeartbeatClient.RequestHeartbeat, NothingInternal, NothingInternal>(request, payload, new NothingInternal());
        }
    }
}
