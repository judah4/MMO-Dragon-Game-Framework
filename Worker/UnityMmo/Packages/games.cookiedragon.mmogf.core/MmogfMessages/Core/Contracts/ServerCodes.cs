namespace Mmogf.Core.Contracts
{
    public enum ServerCodes : byte
    {
        None,
        ClientConnect,
        ServerConnect,
        EntityInfo,
        ChangeInterestArea,
        EntityUpdate,
        WorldCommandRequest,
        WorldCommandResponse,
        EntityCommandRequest,
        EntityCommandResponse,
        EntityEvent,
        EntityDelete,
        EntityCheckout,
        Ping,
    }
}
