using System.Collections;
using System.Collections.Generic;
using MessageProtocols.Core;
using UnityEngine;

public class UpdatePositionReceiver : BaseEntityBehavior
{
    void OnEnable()
    {
        Entity.OnEntityUpdate += OnUpdate;

    }

    void OnDisable()
    {
        Entity.OnEntityUpdate -= OnUpdate;
    }

    void OnUpdate()
    {
        var position = Position.Parser.ParseFrom(Entity.Data[Position.ComponentId]);
        var localPos = Server.PositionToClient(position);

        Debug.Log($"{Entity.EntityId} {position.ToString()} {localPos}");

        transform.position = localPos;


    }

    void Update()
    {

        var pos = Position.Parser.ParseFrom(Entity.Data[Position.ComponentId]);
        var currentPos = Server.PositionToServer(transform.position);
        if (!pos.X.Equals(currentPos.X) || !pos.Y.Equals(currentPos.Y) || !pos.Z.Equals(currentPos.Z))
        {
            Server.UpdateEntity(Entity.EntityId, new PositionUpdate(Server, transform.position).Get());

        }
    }
}
