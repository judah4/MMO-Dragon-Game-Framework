namespace Mmogf.Core
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
