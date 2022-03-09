using MessagePack;
using Mmogf;
using Mmogf.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireVisualizer : BaseEntityBehavior
{

    void OnEnable()
    {
    }

    void Update()
    {
        HandleFireEvents();

        if(Input.GetKeyDown(KeyCode.A))
        {
            Server.SendCommand(Entity.EntityId, Cannon.ComponentId, new Cannon.FireCommand() { Left = true }, response => {
                Debug.Log("Fired Cannon Left!");
            });
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Server.SendCommand(Entity.EntityId, Cannon.ComponentId, new Cannon.FireCommand() { Left = false }, response => {
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
        Debug.Log($"Fire cannon event! Left:{payload.Left}");
    }
}
