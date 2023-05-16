namespace Mmogf.Core
{
    public partial struct Acls
    {
        public bool CanWrite(int componentId, string workerType)
        {
            for(int cnt = 0; cnt < AclList.Count; cnt++)
            {
                var acl = AclList[cnt];
                if(acl.ComponentId != componentId)
                    continue;

                return acl.WorkerType == workerType;
            }
            
            return false;
        }


    }
}
