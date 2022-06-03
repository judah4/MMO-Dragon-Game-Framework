using MessagePack;
using Mmogf;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehavior: BaseEntityBehavior
{

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
    }

    void HandleFire(CommandRequest request, Cannon.FireCommand payload)
    {
        Debug.Log("Got Cannon Fire!");

        Server.SendEvent(request.EntityId, Cannon.ComponentId, new Cannon.FireEvent() { Left = payload.Left });
        //make empty response object
        Server.SendCommandResponse(request, new Cannon.FireCommand());
    }
}
