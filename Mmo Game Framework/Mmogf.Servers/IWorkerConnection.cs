using Mmogf.Servers;
using Mmogf.Servers.Shared;

namespace MmoGameFramework
{
    public interface IWorkerConnection
    {
        public RemoteWorkerIdentifier WorkerId { get; }
        public Position InterestPosition { get; }
        public float InterestRange { get; }
    }
}
