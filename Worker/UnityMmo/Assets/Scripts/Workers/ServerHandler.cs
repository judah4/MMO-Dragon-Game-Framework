using Dragongf.Assets.MmogfMessages.GameLogic;
using Mmogf;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandler : CommonHandler
{
    protected override void OnUpdate()
    {
        //debugging
        if(Input.GetKeyDown(KeyCode.S))
        {
            //create ship
            SendCommand(0, 0, new World.CreateEntity("Ship", new Position() { Y = 1, }, Rotation.Identity, new Dictionary<int, byte[]>() {
                { Cannon.ComponentId, MessagePack.MessagePackSerializer.Serialize(new Cannon()) },
            }), response => {
                Debug.Log($"Create Entity! {response.CommandStatus} - {response.Message}");
            });
        }
    }
}
