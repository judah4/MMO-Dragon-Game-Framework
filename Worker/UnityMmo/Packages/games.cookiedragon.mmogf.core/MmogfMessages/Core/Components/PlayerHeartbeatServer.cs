using MessagePack;
using Mmogf.Core;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct PlayerHeartbeatServer : IEntityComponent
    {

        public static int ComponentId = 5;
        public int GetComponentId() => ComponentId;

        /// <summary>
        /// Once this has 3 missed we delete the player
        /// </summary>
        [Key(0)]
        public int MissedHeartbeats { get; set; }

    }
}