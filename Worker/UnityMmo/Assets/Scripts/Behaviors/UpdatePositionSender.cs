using System.Collections;
using System.Collections.Generic;
using MessageProtocols.Core;
using UnityEngine;

public class UpdatePositionSender : BaseEntityBehavior
{
    void OnEnable()
    {
    }

    void Update()
    {

        var pos = Position.Parser.ParseFrom(Entity.Data[new Position().ComponentId]);
        var currentPos = Server.PositionToServer(transform.position);
        if (!pos.X.Equals(currentPos.X) || !pos.Y.Equals(currentPos.Y) || !pos.Z.Equals(currentPos.Z))
        {
            Server.UpdateEntity(Entity.EntityId, new PositionUpdate(Server, transform.position).Get());

        }
    }
}
