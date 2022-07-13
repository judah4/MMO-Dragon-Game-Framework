using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    public class ClientHandler : CommonHandler
    {
        protected override void Init()
        {
            #if UNITY_EDITOR
            ConnectDelay = 2.5f;
            #endif
        }

        protected override void OnConnect()
        {
            SendCommand<PlayerCreator.ConnectPlayer,ConnectPlayerRequest, NothingInternal>(1, PlayerCreator.ComponentId, 
                new ConnectPlayerRequest() { PlayerId = "Dev" }, response =>
            {
                Debug.Log($"Player connect result! {response.CommandStatus} - {response.Message}");
            });
        }
    }
}
