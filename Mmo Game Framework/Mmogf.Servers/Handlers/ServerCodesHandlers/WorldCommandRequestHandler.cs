using Mmogf.Core.Contracts;
using Mmogf.Core.Contracts.Commands;
using Mmogf.Servers.Handlers.WorldCommands;
using Mmogf.Servers.Serializers;
using System;

namespace Mmogf.Servers.Handlers.ServerCodesHandlers
{
    public sealed class WorldCommandRequestHandler
    {
        public ServerCodes ServerCode => ServerCodes.WorldCommandRequest;

        private const ServerCodes SERVER_CODE_RESPONSE = ServerCodes.WorldCommandResponse;

        private readonly ISerializer _serializer;
        private readonly CreateEntityCommandRequestHandler _createEntityCommandRequestHandler;

        public WorldCommandRequestHandler(ISerializer serializer, CreateEntityCommandRequestHandler createEntityCommandRequestHandler)
        {
            _serializer = serializer;
            _createEntityCommandRequestHandler = createEntityCommandRequestHandler;
        }

        public IMmoMessage Handle(RemoteWorkerIdentifier workerId, MmoMessage mmoMessage)
        {
            var commandRequest = _serializer.Deserialize<CommandRequest>(mmoMessage.Info);

            //LidgrenWorkerConnection worker;
            //if (!_connections.TryGetValue(workerId, out worker))
            //{
            //    //send failure
            //    SendToWorker(worker, new MmoMessage()
            //    {
            //        MessageId = ServerCodes.EntityCommandResponse,
            //        Info = _serializer.Serialize(new CommandResponse(CommandResponseHeader.Create(commandRequest.Header, CommandStatus.InvalidRequest, "No Worker Identified"), null)),
            //    });
            //    return;
            //}
            //
            ////todo: verify sender has permissions from config settings
            //
            //if (worker.ConnectionType != "Dragon-Worker")
            //{
            //    SendToWorker(worker, new MmoMessage()
            //    {
            //        MessageId = ServerCodes.EntityCommandResponse,
            //        Info = _serializer.Serialize(new CommandResponse(CommandResponseHeader.Create(commandRequest.Header, CommandStatus.InvalidRequest, "No permission to create entities."), null)),
            //    });
            //    return;
            //}

            switch (commandRequest.Header.CommandId)
            {
                case World.CreateEntity.CommandId:
                    var createEntity = _serializer.Deserialize<World.CreateEntity>(commandRequest.Payload);

                    var entity = _createEntityCommandRequestHandler.Handle(createEntity.Request.Value);

                    createEntity.Response = new CreateEntityResponse(entity.EntityId);

                    return new MmoMessage<CommandResponse<World.CreateEntity>>()
                    {
                        MessageId = SERVER_CODE_RESPONSE,
                        Info = new CommandResponse<World.CreateEntity>(
                            CommandResponseHeader.Create(commandRequest.Header, CommandStatus.Success, ""),
                            createEntity)
                    };

            }

            throw new NotImplementedException();
        }
    }
}
