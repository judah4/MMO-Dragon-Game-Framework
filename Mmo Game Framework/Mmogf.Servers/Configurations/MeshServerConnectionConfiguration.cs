using Lidgren.Network;

namespace Mmogf.Servers.Configurations
{
    public class MeshServerConnectionConfiguration : IMeshServerConnectionConfiguration
    {
        public NetPeerConfiguration NetPeerConfiguration { get; }

        public MeshServerConnectionConfiguration(NetPeerConfiguration netPeerConfiguration)
        {
            NetPeerConfiguration = netPeerConfiguration;
        }
    }
}

