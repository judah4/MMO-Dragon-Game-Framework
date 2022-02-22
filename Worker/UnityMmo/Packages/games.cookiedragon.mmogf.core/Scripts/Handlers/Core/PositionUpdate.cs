using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{

    public class PositionUpdate
    {
        private Position _position;
        private CommonHandler _server;

        public PositionUpdate(CommonHandler server, Vector3 position)
        {
            _server = server;
            _position = _server.PositionToServer(position);
        }

        public Position Get()
        {
            return _position;
        }
    }
}