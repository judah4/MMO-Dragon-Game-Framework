using Mmogf;
using Mmogf.Core;
using Mmogf.Core.Contracts.Commands;
using Mmogf.Core.Contracts.Events;
using UnityEngine;

public class FireBehavior : BaseEntityBehavior
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
        if (!Entity.HasAuthority(Cannon.ComponentId))
            return;

        for (int cnt = 0; cnt < Server.CommandRequests.Count; cnt++)
        {
            var request = Server.CommandRequests[cnt];
            if (request.Header.ComponentId != Cannon.ComponentId)
                continue;
            if (request.Header.EntityId != Entity.EntityId)
                continue;
            //we need a way to identify what command this is... Components will be able to have more commands
            //use ids!
            switch (request.Header.CommandId)
            {
                case Cannon.FireCommand.CommandId:
                    var payload = Server.Serializer.Deserialize<Cannon.FireCommand>(request.Payload);
                    HandleFire(request, payload);
                    break;
            }

        }

        HandleFireEvents();
    }

    void HandleFire(CommandRequest request, Cannon.FireCommand payload)
    {
        Server.SendEvent(request.Header.EntityId, Cannon.ComponentId, new Cannon.FireEvent() { Left = payload.Request?.Left ?? false });
        //make empty response object
        Server.SendCommandResponse<Cannon.FireCommand, FireCommandRequest, Nothing>(request, payload, new Nothing());
    }

    private void HandleFireEvents()
    {
        for (int cnt = 0; cnt < Server.EventRequests.Count; cnt++)
        {
            var request = Server.EventRequests[cnt];

            if (request.Header.ComponentId != Cannon.ComponentId)
                continue;
            if (request.Header.EntityId != Entity.EntityId)
                continue;

            switch (request.Header.EventId)
            {
                case Cannon.FireEvent.EventId:
                    var payload = Server.Serializer.Deserialize<Cannon.FireEvent>(request.Payload);
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
