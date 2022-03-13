using MessagePack;
using Mmogf.Core;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct PlayerHeartbeatClient : IEntityComponent
    {

        public static int ComponentId = 6;
        public int GetComponentId() => ComponentId;

        [MessagePackObject]
        public struct RequestHeartbeat : ICommand
        {
            public const int CommandId = 102;
            public int GetCommandId() => CommandId;

        }

    }
}