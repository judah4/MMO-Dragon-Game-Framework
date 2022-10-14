using MessagePack;
using Mmogf;
using Mmogf.Core;
using System;
using System.Collections;
using System.Collections.Generic;
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
        if(health.HasValue && health.Value.Current < 1)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Server.SendCommand<Cannon.FireCommand,FireCommandRequest,Nothing>(Entity.EntityId, Cannon.ComponentId, new FireCommandRequest() { Left = true }, result => {
                if(result.CommandStatus != CommandStatus.Success)
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
            Server.SendCommand<Cannon.FireCommand, FireCommandRequest, Nothing>(Entity.EntityId, Cannon.ComponentId,  new FireCommandRequest() { Left = false }, result => {
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
            if (Server.EventRequests[cnt].ComponentId != Cannon.ComponentId)
                continue;
            if(Server.EventRequests[cnt].EntityId != Entity.EntityId)
                continue;

            var request = Server.EventRequests[cnt];
            //we need a way to identify what command this is... Components will be able to have more commands
            //use ids!
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

        _cannonFiring.SpawnCannonball(payload.Left);
    }
}
