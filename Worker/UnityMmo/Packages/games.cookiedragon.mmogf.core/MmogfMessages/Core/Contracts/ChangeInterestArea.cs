using System.Runtime.Serialization;

namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct ChangeInterestArea
    {
        [DataMember(Order = 1)]
        public FixedVector3 Position { get; set; }

    }
}