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

        [IgnoreMember]
        public Rotation Rotation
        {
            get { return MessagePackSerializer.Deserialize<Rotation>(EntityData[Rotation.ComponentId]); }
        }

    }
}
