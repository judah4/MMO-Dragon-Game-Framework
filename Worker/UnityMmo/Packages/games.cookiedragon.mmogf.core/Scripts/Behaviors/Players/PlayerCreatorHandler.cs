using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    public class PlayerCreatorHandler : BaseEntityBehavior
    {

        public static System.Func<PlayerCreator.ConnectPlayer, CommandRequest, World.CreateEntity> CreatePlayer;

        private void Update()
        {
            for (int cnt = 0; cnt < Server.CommandRequests.Count; cnt++)
            {
                var request = Server.CommandRequests[cnt];

                Debug.Log($"{request.CommandId} {request.ComponentId}");
                if (Server.CommandRequests[cnt].ComponentId != PlayerCreator.ComponentId)
                    continue;
                //we need a way to identify what command this is... Components will be able to have more commands
                //use ids!
                switch (request.CommandId)
                {
                    case PlayerCreator.ConnectPlayer.CommandId:
                        HandleConnect(request);
                        break;
                }

            }
        }

        void HandleConnect(CommandRequest request)
        {
            Debug.Log("Got player connect!");
            var connectPlayer = MessagePack.MessagePackSerializer.Deserialize<PlayerCreator.ConnectPlayer>(request.Payload);
            var createPayload = CreatePlayer.Invoke(connectPlayer, request);
            Server.SendCommand(0, 0, createPayload,
                response => {
                    Debug.Log($"Create played! {response.CommandStatus} - {response.Message}");
                });

            //make empty response object
            Server.SendCommandResponse(request, new PlayerCreator.ConnectPlayer());
        }
    }
}
