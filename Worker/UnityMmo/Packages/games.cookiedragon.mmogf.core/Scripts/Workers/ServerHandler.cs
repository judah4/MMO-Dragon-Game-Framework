using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    public class ServerHandler : CommonHandler
    {
        public static System.Action<ServerHandler> OnConnectServer;

        protected override void Init()
        {
#if !UNITY_EDITOR
            LockFramerate();
#endif
        }

        protected override void OnConnect()
        {
            OnConnectServer?.Invoke(this);
        }

        public static void LockFramerate()
        {
            if (Application.isEditor == false)
            {
                Application.targetFrameRate = 30;
            }

            Time.fixedDeltaTime = 1 / 30f;
            QualitySettings.vSyncCount = 0;
        }

    }

}
