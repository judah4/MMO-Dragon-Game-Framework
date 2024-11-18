using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct PlayerHeartbeatServer : IEntityComponent
    {

        public const short ComponentId = 5;
        public short GetComponentId() => ComponentId;

        /// <summary>
        /// Once this has 3 missed we delete the player
        /// </summary>
        [Key(0)]
        public short MissedHeartbeats { get; set; }

    }
}