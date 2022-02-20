using MessagePack;
using System;

namespace Mmogf.Core
{
    public partial struct EntityInfo
    {
        [IgnoreMember]
        public Position Position
        {
            get { return MessagePackSerializer.Deserialize<Position>(EntityData[Position.ComponentId]); }
        }
       
    }
}
