using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Core
{
    public struct CommandResult<TCommand, TRequest, TResponse> where TCommand : ICommandBase<TRequest,TResponse> where TRequest : struct where TResponse : struct
    {

    }
}
