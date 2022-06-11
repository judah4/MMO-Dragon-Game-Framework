using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Core
{
    public interface ICommandBase<TRequest, TResponse> : ICommand where TRequest : struct where TResponse : struct
    {
        public TRequest? Request { get; set; }
        public TResponse? Response { get; set; }
    }
}
