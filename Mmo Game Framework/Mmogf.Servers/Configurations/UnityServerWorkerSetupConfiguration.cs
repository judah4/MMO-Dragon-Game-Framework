using Lidgren.Network;

namespace Mmogf.Servers.Configurations
{
    public class UnityServerWorkerSetupConfiguration : IWorkerSetupConfiguration
    {
        public int InterestRange => 2000;

        public NetPeerConfiguration NetPeerConfiguration { get; }

        public UnityServerWorkerSetupConfiguration(NetPeerConfiguration netPeerConfiguration)
        {
            NetPeerConfiguration = netPeerConfiguration;
        }
    }
}
