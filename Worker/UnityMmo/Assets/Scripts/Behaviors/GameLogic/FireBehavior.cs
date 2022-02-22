using Assets.Scripts.Helpers;
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

            var request = Server.CommandRequests[cnt];

            //we need a way to identify what command this is... Components will be able to have more commands

            HandleFire(request);
        }
    }

    void HandleFire(CommandRequest request)
    {
        Debug.Log("Got Cannon Fire!");
        //make empty response object
        Server.SendCommandResponse(request, new Cannon.FireCommand());
    }
}
