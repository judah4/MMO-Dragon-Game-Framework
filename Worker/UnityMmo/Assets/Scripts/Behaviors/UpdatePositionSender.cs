using System.Collections;
using System.Collections.Generic;
using MessageProtocols.Core;
using UnityEngine;

public class UpdatePositionSender : BaseEntityBehavior
{
    private float updateTime = 0;
    private float updateTick = .1f;

    void OnEnable()
    {
    }

    

    void Update()
    {
        if(Time.time - updateTime < updateTick)
            return;

        var pos = Position.Parser.ParseFrom(Entity.Data[PositionComponent.ComponentId]);
        var currentPos = Server.PositionToClient(pos);

        if (pos.Y < -100)
        {
            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = true;
                
            }

            pos.Y = -100;
            transform.position = Server.PositionToClient(pos);
        }


        if (Mathf.Abs((currentPos - transform.position).sqrMagnitude) > .22f)
        {
            Server.UpdateEntity(Entity.EntityId, PositionComponent.ComponentId, new PositionUpdate(Server, transform.position).Get());
            updateTime = Time.time;
        }
    }
}
