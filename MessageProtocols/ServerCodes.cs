using System;
using System.Collections.Generic;
using System.Text;

namespace MessageProtocols
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
    }
}
