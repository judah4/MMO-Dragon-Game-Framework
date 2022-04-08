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
            ConnectDelay = 5f;
            #endif
        }

        protected override void OnConnect()
        {
            SendCommand(1, PlayerCreator.ComponentId, new PlayerCreator.ConnectPlayer() { PlayerId = "Dev", }, response =>
            {
                Debug.Log($"Player connect! {response.CommandStatus} - {response.Message}");
            });
        }
    }
}
