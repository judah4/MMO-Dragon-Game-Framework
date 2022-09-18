using System;
using System.Collections.Generic;
using System.Text;
using Mmogf.Core;

namespace MmoGameFramework
{
    public class WorkerConnection
    {
        public string ConnectionType { get; set; }
        public Position InterestPosition { get; set; }
        public float InterestRange { get; set; }
        public Lidgren.Network.NetConnection Connection { get; set; }

        public HashSet<int> EntitiesInRange { get; set; }

        public WorkerConnection(string connectionType, Lidgren.Network.NetConnection senderConnection, Position interestPosition)
        {
            ConnectionType = connectionType;
            InterestPosition = interestPosition;
            InterestRange = 500;
            Connection = senderConnection;
            EntitiesInRange = new HashSet<int>();
        }

    }
}
