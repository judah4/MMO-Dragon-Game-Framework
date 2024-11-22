using Mmogf.Core.Contracts.Commands;
using System.Threading.Tasks;

namespace Mmogf.Servers
{
    public interface ICommandHandler
    {
        Task<CommandResponse> HandleEntityEvent(RemoteWorkerIdentifier workerId, CommandRequest eventRequest);

    }
}
