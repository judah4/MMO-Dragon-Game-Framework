using System;
using System.Collections.Generic;
using System.Text;

namespace Mmogf.Core
{
    public enum ServerCodes
    {
        None,
        ClientConnect,
        ServerConnect,
        EntityInfo,
        GameData,
        ChangeInterestArea,
        EntityUpdate,
        EntityCommandRequest,
        EntityCommandResponse,
        Ping,
    }
}
