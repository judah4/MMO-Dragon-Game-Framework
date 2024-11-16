using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Mmogf.Servers.Contracts
{
    /// <summary>
    /// Access Control List. Component for tracking data ownership for clients and server workers.
    /// </summary>
    [DataContract]
    public struct Acls : IEntityComponent
    {
        public const short ComponentId = 4;
        public short GetComponentId() { return ComponentId; }

        [DataMember(Order = 0)]
        public List<Acl> AclList { get; set; }

        public bool CanWrite(int componentId, string workerType)
        {
            for (int cnt = 0; cnt < AclList.Count; cnt++)
            {
                var acl = AclList[cnt];
                if (acl.ComponentId != componentId)
                    continue;

                return acl.WorkerType == workerType;
            }

            return false;
        }
    }
}
