using Lidgren.Network;

namespace Mmogf.Servers.Configurations
{
    public interface IMeshServerConnectionConfiguration
    {
        NetPeerConfiguration NetPeerConfiguration { get; }
    }
}