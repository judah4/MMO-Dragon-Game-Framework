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
        var currentPos = Server.PositionToClient(pos);
        if (Mathf.Abs((currentPos - transform.position).sqrMagnitude) > .01f)
        {
            Server.UpdateEntity(Entity.EntityId, new PositionUpdate(Server, transform.position).Get());

        }
    }
}
