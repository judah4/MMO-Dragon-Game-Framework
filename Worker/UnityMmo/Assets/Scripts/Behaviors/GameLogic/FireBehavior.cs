using MessagePack;
using Mmogf;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehavior: BaseEntityBehavior
{

    [SerializeField]
    private CannonFiring _cannonFiring;

    //get command requests

    void OnEnable()
    {
        //register a callback?
        //Entity.OnCommandResponse

    }

    private void Update()
    {
        for (int cnt = 0; cnt < Server.CommandRequests.Count; cnt++)
        {
            if (Server.CommandRequests[cnt].ComponentId != Cannon.ComponentId)
                continue;
            if (Server.CommandRequests[cnt].EntityId != Entity.EntityId)
                continue;
            var request = Server.CommandRequests[cnt];
            //we need a way to identify what command this is... Components will be able to have more commands
            //use ids!
            switch (request.CommandId)
            {
                case Cannon.FireCommand.CommandId:
                    var payload = MessagePackSerializer.Deserialize<Cannon.FireCommand>(request.Payload);
                    HandleFire(request, payload);
                    break;
            }

        }

        HandleFireEvents();
    }

    void HandleFire(CommandRequest request, Cannon.FireCommand payload)
    {
        Server.SendEvent(request.EntityId, Cannon.ComponentId, new Cannon.FireEvent() { Left = payload.Request?.Left ?? false });
        //make empty response object
        Server.SendCommandResponse<Cannon.FireCommand, FireCommandRequest, Nothing>(request, payload, new Nothing());
    }

    private void HandleFireEvents()
    {
        for (int cnt = 0; cnt < Server.EventRequests.Count; cnt++)
        {
            if (Server.EventRequests[cnt].ComponentId != Cannon.ComponentId)
                continue;
            if (Server.EventRequests[cnt].EntityId != Entity.EntityId)
                continue;

            var request = Server.EventRequests[cnt];
            switch (request.EventId)
            {
                case Cannon.FireEvent.EventId:
                    var payload = MessagePackSerializer.Deserialize<Cannon.FireEvent>(request.Payload);
                    HandleFireEvent(request, payload);
                    break;
            }

        }
    }

    private void HandleFireEvent(EventRequest request, Cannon.FireEvent payload)
    {

        var cannonBall = _cannonFiring.SpawnCannonball(payload.Left);

        cannonBall.InitServer(this);
    }
}
