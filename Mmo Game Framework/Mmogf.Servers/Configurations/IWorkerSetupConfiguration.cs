using Lidgren.Network;

namespace Mmogf.Servers.Configurations
{
    public interface IWorkerSetupConfiguration
    {
        public int InterestRange { get; }

        public NetPeerConfiguration NetPeerConfiguration { get; }

    }
}
