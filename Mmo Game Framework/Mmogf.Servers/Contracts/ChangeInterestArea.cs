using System.Runtime.Serialization;

namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct ChangeInterestArea
    {
        [DataMember(Order = 0)]
        public FixedVector3 Position { get; set; }

    }
}