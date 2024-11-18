using Lidgren.Network;
using Mmogf.Core.Contracts;
using Mmogf.Servers.Shared;

namespace MmoGameFramework
{
    public interface IServerTransportation
    {
        /// <summary>
        /// Send a message to a connected client.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        /// <param name="deliveryMethod"></param>
        void SendToWorker(WorkerConnection connection, MmoMessage message, NetDeliveryMethod deliveryMethod);
        /// <summary>
        /// Send a message to clients that are subscribbed to the specified entity.
        /// </summary>
        /// <param name="entitySubscribedTo"></param>
        /// <param name="message"></param>
        /// <param name="workerIdToExclude"></param>
        /// <param name="deliveryMethod"></param>
        void SendSubscribed(Entity entitySubscribedTo, MmoMessage message, RemoteWorkerIdentifier workerIdToExclude, NetDeliveryMethod deliveryMethod);

        /// <summary>
        /// Start the network
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the network
        /// </summary>
        void Stop();
    }
}