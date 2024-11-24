using Lidgren.Network;

namespace Mmogf.Servers.Configurations
{
    public class UnityServerWorkerConnectionConfiguration : IWorkerConnectionConfiguration
    {
        public int InterestRange => 2000;

        public NetPeerConfiguration NetPeerConfiguration { get; }

        public UnityServerWorkerConnectionConfiguration(NetPeerConfiguration netPeerConfiguration)
        {
            NetPeerConfiguration = netPeerConfiguration;
        }
    }
}
