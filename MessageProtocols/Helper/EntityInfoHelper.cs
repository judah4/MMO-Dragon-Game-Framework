using System;
using MessageProtocols.Core;

namespace MessageProtocols.Server
{
    public partial class EntityInfo
    {
        public Position Position
        {
            get { return Core.Position.Parser.ParseFrom(EntityData[new Position().ComponentId]); }
        }
       
    }
}
