using Lidgren.Network;
using Mmogf.Core.Contracts;

namespace Mmogf.Servers
{
    internal interface IClientTransportation
    {
        /// <summary>
        /// Send a message to a connected client.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        /// <param name="deliveryMethod"></param>
        void Send(MmoMessage message, NetDeliveryMethod deliveryMethod);
    }
}