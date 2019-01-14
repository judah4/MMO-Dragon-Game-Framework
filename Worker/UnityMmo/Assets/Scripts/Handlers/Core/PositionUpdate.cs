using System.Collections;
using System.Collections.Generic;
using MessageProtocols.Core;
using UnityEngine;

public class PositionUpdate
{
    private Position _position;
    private CommonHandler _server;

    public PositionUpdate(CommonHandler server, Vector3 position)
    {
        _server = server;
        _position = server.PositionToServer(position);
    }

    public Position Get()
    {
        return _position;
    }
}
