using MessagePack;
using UnityEngine;

namespace Mmogf.Core
{
    public class PlayerCreatorHandler : BaseEntityBehavior
    {

        public static System.Func<PlayerCreator.ConnectPlayer, CommandRequest, CreateEntityRequest> CreatePlayer;

        private void Update()
        {
            if(!HasAuthority(PlayerCreator.ComponentId))
                return;

            for (int cnt = 0; cnt < Server.CommandRequests.Count; cnt++)
            {
                var request = Server.CommandRequests[cnt];

                if (Server.CommandRequests[cnt].EntityId != Entity.EntityId)
                    continue;
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
            Debug.Log($"Got player connect! {request.RequesterId} {request.RequestorWorkerType}");
            var connectPlayer = MessagePack.MessagePackSerializer.Deserialize<PlayerCreator.ConnectPlayer>(request.Payload);
            var createPayload = CreatePlayer.Invoke(connectPlayer, request);
            Server.SendWorldCommand<World.CreateEntity,CreateEntityRequest, NothingInternal>(createPayload,
                response => {
                    Debug.Log($"Create played! {response.CommandStatus} - {response.Message}");
                });

            var payload = MessagePackSerializer.Deserialize<PlayerCreator.ConnectPlayer>(request.Payload);

            //make empty response object
            Server.SendCommandResponse<PlayerCreator.ConnectPlayer,ConnectPlayerRequest, NothingInternal>(request, payload, new NothingInternal());
        }
    }
}
