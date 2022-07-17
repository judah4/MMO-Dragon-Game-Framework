using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    public class ServerHandler : CommonHandler
    {

        protected override void Init()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
        }

        protected override void OnConnect()
        {
        }
    }

}
