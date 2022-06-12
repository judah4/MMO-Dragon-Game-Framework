using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Mmogf
{

    public interface ICommandBase<TRequest, TResponse> : ICommand where TRequest : struct where TResponse : struct
    {
        TRequest? Request { get; set; }
        TResponse? Response { get; set; }
    }

    /// <summary>
    /// Fires to server and expects a response
    /// </summary>
    public interface ICommand : IMessage
    {
        int GetCommandId();
    }

    /// <summary>
    /// Fires of data to servers and clients
    /// </summary>
    public interface IEvent : IMessage
    {
        int GetEventId();
    }

    public interface IEntityComponent : IMessage
    {
        int GetComponentId();
    }
}
