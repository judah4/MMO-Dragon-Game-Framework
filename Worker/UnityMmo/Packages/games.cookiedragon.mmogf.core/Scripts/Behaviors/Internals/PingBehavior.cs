using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mmogf.Core.Behaviors
{
    internal interface IInternalBehavior
    {
        void Update();
    }

    internal class PingBehavior : IInternalBehavior
    {
        public float Timer = 5f;
        MmoWorker _worker;

        public PingBehavior(MmoWorker worker)
        {
            _worker = worker;
        }

        public void Update()
        {
            Timer -= Time.deltaTime;

            if(Timer <= 0)
            {
                Timer = 5;
                _worker.SendPing();
            }
        }
    }
}
