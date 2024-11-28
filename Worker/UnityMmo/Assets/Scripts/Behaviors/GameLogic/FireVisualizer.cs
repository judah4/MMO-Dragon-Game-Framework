using Mmogf;
using Mmogf.Core;
using Mmogf.Core.Contracts.Commands;
using Mmogf.Core.Contracts.Events;
using UnityEngine;

public class FireVisualizer : BaseEntityBehavior
{

    [SerializeField]
    private CannonFiring _cannonFiring;

    void OnEnable()
    {
    }

    void Update()
    {
        HandleFireEvents();

        if (!Entity.HasAuthority(ClientAuthCheck.ComponentId))
            return;

        var health = GetEntityComponent<Health>();
        if (health.HasValue && health.Value.Current < 1)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Server.SendCommand<Cannon.FireCommand, FireCommandRequest, Nothing>(Entity.EntityId, Cannon.ComponentId, new FireCommandRequest() { Left = true }, result =>
            {
                if (result.CommandStatus != CommandStatus.Success)
                {
                    Debug.LogError($"{result.CommandId}: {result.CommandStatus} - {result.Message}");
                    return;
                }
                //how should we check the response?
                Debug.Log("Fired Cannon Left!");
            });
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Server.SendCommand<Cannon.FireCommand, FireCommandRequest, Nothing>(Entity.EntityId, Cannon.ComponentId, new FireCommandRequest() { Left = false }, result =>
            {
                if (result.CommandStatus != CommandStatus.Success)
                {
                    Debug.LogError($"{result.CommandId}: {result.CommandStatus} - {result.Message}");
                    return;
                }
                Debug.Log("Fired Cannon Right!");
            });
        }
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

            //we need a way to identify what command this is... Components will be able to have more commands
            //use ids!
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

        _cannonFiring.SpawnCannonball(payload.Left);
    }
}
