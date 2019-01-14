using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Google.Protobuf;

namespace MessageProtocols
{
    public interface IEntityComponent
    {
        int ComponentId { get; }
    }
}
