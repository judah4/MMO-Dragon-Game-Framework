namespace Mmogf.Servers.Contracts
{

    public interface ICommandBase<TRequest, TResponse> : ICommand where TRequest : struct where TResponse : struct
    {
        TRequest? Request { get; set; }
        TResponse? Response { get; set; }
    }

    /// <summary>
    /// Fires to server and expects a response
    /// </summary>
    public interface ICommand
    {
        short GetCommandId();
    }

    /// <summary>
    /// Fires off data to servers and clients
    /// </summary>
    public interface IEvent
    {
        short GetEventId();
    }

    /// <summary>
    /// Entity data
    /// </summary>
    public interface IEntityComponent
    {
        short GetComponentId();
    }
}
