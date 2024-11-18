using MessagePack;

namespace Mmogf.Core.Contracts
{
    public enum AclAccess
    {
        None,
        Read,
        Write,
    }

    /// <summary>
    /// Access Control List
    /// </summary>
    [MessagePackObject]
    public struct Acl
    {
        [Key(0)]
        public short ComponentId { get; set; }

        [Key(1)]
        public string WorkerType { get; set; }

        [Key(2)]
        public long WorkerId { get; set; }
    }
}