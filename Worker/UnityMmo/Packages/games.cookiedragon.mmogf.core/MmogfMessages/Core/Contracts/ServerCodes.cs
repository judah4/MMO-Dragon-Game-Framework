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
        EntityCommandRequest,
        EntityCommandResponse,
        Ping,
        EntityEvent,
        EntityDelete,
        EntityCheckout,
    }
}
