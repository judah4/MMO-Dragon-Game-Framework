using System;
using System.Collections.Generic;
using System.Text;
using MessageProtocols.Core;

namespace MmoGameFramework
{
    public class WorkerConnection
    {
        public string ConnectionType { get; set; }
        public Position InterestPosition { get; set; }
        public float InterestRange { get; set; }

        public WorkerConnection(string connectionType, Position interestPosition)
        {
            ConnectionType = connectionType;
            InterestPosition = interestPosition;
            InterestRange = 100;
        }

    }
}
