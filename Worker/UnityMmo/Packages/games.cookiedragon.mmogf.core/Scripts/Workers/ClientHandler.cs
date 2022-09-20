using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    public class ClientHandler : CommonHandler
    {
        [SerializeField]
        private string _testPlayerId = "Dev";
        [SerializeField]
        bool _useTestId = false;

        public static System.Action<ClientHandler> OnConnectClient;


        protected override void Init()
        {
            #if UNITY_EDITOR
            ConnectDelay = 2.5f;
            #endif
        }

        protected override void OnConnect()
        {
            #if !UNITY_EDITOR
            _useTestId = false;
            #endif

            var playerId = Guid.NewGuid().ToString();
            if(_useTestId)
            {
                playerId = _testPlayerId;
            }

            SendCommand<PlayerCreator.ConnectPlayer,ConnectPlayerRequest, NothingInternal>(1, PlayerCreator.ComponentId, 
                new ConnectPlayerRequest() { PlayerId = playerId }, response =>
            {
                Debug.Log($"Player connect result! {response.CommandStatus} - {response.Message}");
            });

            OnConnectClient?.Invoke(this);
        }
    }
}
