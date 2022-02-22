using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Mmogf
{
    public interface IEntityComponent : IMessage
    {
        int GetComponentId();
    }
}
