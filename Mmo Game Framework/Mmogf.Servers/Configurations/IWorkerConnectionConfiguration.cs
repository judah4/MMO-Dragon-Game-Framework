using Lidgren.Network;

namespace Mmogf.Servers.Configurations
{
    public interface IWorkerConnectionConfiguration
    {
        public int InterestRange { get; }

        public NetPeerConfiguration NetPeerConfiguration { get; }
    }
}
