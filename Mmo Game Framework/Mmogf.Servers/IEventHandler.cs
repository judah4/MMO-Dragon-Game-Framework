using Mmogf.Core.Contracts.Events;
using System.Threading.Tasks;

namespace Mmogf.Servers
{
    public interface IEventHandler
    {
        Task HandleEntityEvent(RemoteWorkerIdentifier workerId, EventRequest eventRequest);
    }
}
