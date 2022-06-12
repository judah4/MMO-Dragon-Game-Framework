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
        public struct RequestHeartbeat : ICommandBase<NothingInternal, NothingInternal>
        {
            public const int CommandId = 102;
            public int GetCommandId() => CommandId;


            [Key(0)]
            public NothingInternal? Request { get; set; }
            [Key(1)]
            public NothingInternal? Response { get; set; }

        }

    }
}