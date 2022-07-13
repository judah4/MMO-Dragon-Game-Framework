using MessagePack;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class HealthBehavior : BaseEntityBehavior
    {
        private void Update()
        {
            for (int cnt = 0; cnt < Server.CommandRequests.Count; cnt++)
            {
                if (Server.CommandRequests[cnt].ComponentId != Health.ComponentId)
                    continue;
                if (Server.CommandRequests[cnt].EntityId != Entity.EntityId)
                    continue;
                var request = Server.CommandRequests[cnt];
                //we need a way to identify what command this is... Components will be able to have more commands
                //use ids!
                switch (request.CommandId)
                {
                    case Health.TakeDamageCommand.CommandId:
                        var payload = MessagePackSerializer.Deserialize<Health.TakeDamageCommand>(request.Payload);
                        HandleTakeDamage(request, payload);
                        break;
                }

            }

        }

        void HandleTakeDamage(CommandRequest request, Health.TakeDamageCommand payload)
        {
            if(payload.Request.HasValue == false)
            {
                Server.SendCommandResponseFailure(request, "No request payload!");
                return;
            }

            var health = (Health)Entity.Data[Health.ComponentId];

            bool wasAlive = health.Current > 0;

            health.Current -= payload.Request.Value.Amount;

            Server.UpdateEntity(Entity.EntityId, Health.ComponentId, health);

            var dead = health.Current < 1;
            var killed = dead && wasAlive;

            //make empty response object
            Server.SendCommandResponse<Health.TakeDamageCommand, TakeDamageRequest, TakeDamageResponse>(request, payload, new TakeDamageResponse() { Dead = dead, Killed = killed });
        }

    }
}
