using Mmogf.Servers.Contracts.Commands;
using Mmogf.Servers.Shared;
using System.Threading.Tasks;

namespace Mmogf.Servers
{
    public interface ICommandHandler
    {
        Task<CommandResponse> HandleEntityEvent(RemoteWorkerIdentifier workerId, CommandRequest eventRequest);

    }
}
