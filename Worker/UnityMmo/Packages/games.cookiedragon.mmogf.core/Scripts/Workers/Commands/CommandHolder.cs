using Mmogf.Core.Contracts;
using Mmogf.Core.Contracts.Commands;
using Mmogf.Servers.Serializers;
using System;
using System.Collections.Generic;

namespace Mmogf.Core
{
    public class CommandHolder
    {
        public CommandRequest Request { get; set; }

        public float TimeoutTimer { get; set; }

        public CommandHolder()
        {
            TimeoutTimer = 1000;
        }

        public CommandHolder(CommandRequest request, float timeoutTimer)
        {
            Request = request;
            TimeoutTimer = timeoutTimer;
        }

        public virtual void SendResponse(CommandResponse response)
        {

        }
        public virtual void CleanUp()
        {

        }

    }

    public class CommandHolderTyped<TCommand, TRequest, TResponse> : CommandHolder where TCommand : ICommandBase<TRequest, TResponse> where TRequest : struct where TResponse : struct
    {
        public ICommandBase<TRequest, TResponse> Command { get; set; }
        public Action<CommandResult<TCommand, TRequest, TResponse>> OnResponse { get; set; }

        private ISerializer _serializer;

        public CommandHolderTyped(ISerializer serializer) : base()
        {
            _serializer = serializer;
        }

        public CommandHolderTyped(ISerializer serializer, CommandRequest request, ICommandBase<TRequest, TResponse> command, Action<CommandResult<TCommand, TRequest, TResponse>> response, float timeoutTimer) : base(request, timeoutTimer)
        {
            OnResponse = response;
            Command = command;
        }

        public override void SendResponse(CommandResponse response)
        {

            TRequest? requestPayload = null;
            TResponse? responsePayload = null;
            if (response.Payload != null)
            {
                var command = _serializer.Deserialize<TCommand>(response.Payload);
                requestPayload = command.Request;
                responsePayload = command.Response;
            }

            var result = CommandResult<TCommand, TRequest, TResponse>.Create(response, requestPayload, responsePayload);
            OnResponse?.Invoke(result);
        }

        public void Setup(CommandRequest request, ICommandBase<TRequest, TResponse> command, Action<CommandResult<TCommand, TRequest, TResponse>> response, float timeoutTimer)
        {
            OnResponse = response;
            Command = command;
            Request = request;
            TimeoutTimer = timeoutTimer;
        }

        public override void CleanUp()
        {
            OnResponse = null;
        }

    }

    public class CommandHolderCache
    {
        public struct CommandIdGroup
        {
            public int ComponentId { get; set; }
            public int CommandId { get; set; }
        }

        Dictionary<(int componentId, int commandId), List<CommandHolder>> _commandsCache = new Dictionary<(int componentId, int commandId), List<CommandHolder>>();

        public CommandHolderTyped<TCommand, TRequest, TResponse> Get<TCommand, TRequest, TResponse>(ISerializer serializer, CommandRequest request, ICommandBase<TRequest, TResponse> command, Action<CommandResult<TCommand, TRequest, TResponse>> response, float timeoutTimer) where TCommand : ICommandBase<TRequest, TResponse> where TRequest : struct where TResponse : struct
        {
            var commandKey = (request.Header.ComponentId, request.Header.CommandId);

            List<CommandHolder> list;
            if (!_commandsCache.TryGetValue(commandKey, out list))
            {
                list = new List<CommandHolder>();
                _commandsCache.Add(commandKey, list);
            }

            CommandHolderTyped<TCommand, TRequest, TResponse> commandHolder;
            if (list.Count > 0)
            {
                commandHolder = list[0] as CommandHolderTyped<TCommand, TRequest, TResponse>;
                list.RemoveAt(0);
            }
            else
            {
                commandHolder = new CommandHolderTyped<TCommand, TRequest, TResponse>(serializer);
            }

            commandHolder.Setup(request, command, response, timeoutTimer);

            return commandHolder;
        }

        public void Release(CommandHolder commandHolder)
        {
            var commandKey = (commandHolder.Request.Header.ComponentId, commandHolder.Request.Header.CommandId);
            List<CommandHolder> list;
            if (!_commandsCache.TryGetValue(commandKey, out list))
            {
                list = new List<CommandHolder>();
                _commandsCache.Add(commandKey, list);
            }

            commandHolder.CleanUp();
            list.Add(commandHolder);
        }

    }

}
