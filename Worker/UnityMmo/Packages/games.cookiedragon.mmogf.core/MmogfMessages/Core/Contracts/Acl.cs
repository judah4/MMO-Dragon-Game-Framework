using System.Runtime.Serialization;

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
    [DataContract]
    public struct Acl
    {
        [DataMember(Order = 0)]
        public short ComponentId { get; set; }

        [DataMember(Order = 1)]
        public string WorkerType { get; set; }

        [DataMember(Order = 2)]
        public long WorkerId { get; set; }
    }
}