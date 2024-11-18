using Mmogf.Core.Contracts.Events;
using Mmogf.Servers.Shared;
using System.Threading.Tasks;

namespace Mmogf.Servers
{
    public interface IEventHandler
    {
        Task HandleEntityEvent(RemoteWorkerIdentifier workerId, EventRequest eventRequest);
    }
}
