using MessagePack;
using Mmogf.Core;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct PlayerHeartbeatServer : IEntityComponent
    {

        public static short ComponentId = 5;
        public short GetComponentId() => ComponentId;

        /// <summary>
        /// Once this has 3 missed we delete the player
        /// </summary>
        [Key(0)]
        public short MissedHeartbeats { get; set; }

    }
}