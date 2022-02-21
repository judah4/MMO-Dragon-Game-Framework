using Assets.Scripts.Helpers;
using MessagePack;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePositionSender : BaseEntityBehavior
{
    private float updateTime = 0;
    private float updateTick = .1f;

    public bool UpdateRotation = true;

    void OnEnable()
    {
    }

    

    void Update()
    {
        if(Time.time - updateTime < updateTick)
            return;


        var rot = MessagePackSerializer.Deserialize<Rotation>(Entity.Data[Rotation.ComponentId]);
        if(UpdateRotation) 
        { 
            var currentRot = rot.ToQuaternion();
            if(currentRot != transform.rotation)
            {
                Server.UpdateEntity(Entity.EntityId, Rotation.ComponentId, transform.rotation.ToRotation());
            }
        }
        var pos = MessagePackSerializer.Deserialize<Position>(Entity.Data[Position.ComponentId]);

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
            //send destroy at some point
        }

        if (Mathf.Abs((currentPos - transform.position).sqrMagnitude) > .22f)
        {
            Server.UpdateEntity(Entity.EntityId, Position.ComponentId, new PositionUpdate(Server, transform.position).Get());
            updateTime = Time.time;
        }
    }
}
