using MessagePack;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    public class UpdatePositionSender : BaseEntityBehavior
    {
        private float updateTime = -1;
        private float updateTimeRotation = -1;
        private float updateTick = .12f;

        public bool UpdateRotation = true;

        void OnEnable()
        {
        }

        void Update()
        {
            updateTime -= Time.deltaTime;
            updateTimeRotation -= Time.deltaTime;

            if(updateTimeRotation <= 0)
            {
                var rot = GetEntityComponent<Rotation>(Rotation.ComponentId).Value;
                if (UpdateRotation)
                {
                    var currHeading = transform.rotation.ToRotation();
                    if (rot.Heading != currHeading.Heading)
                    {
                        Server.UpdateEntity(Entity.EntityId, Rotation.ComponentId, currHeading);
                        updateTimeRotation = updateTick;
                    }
                }
            }

            if (updateTime > 0)
                return;

            var pos = GetEntityComponent<FixedVector3>(FixedVector3.ComponentId).Value.ToPosition();

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

            if (Mathf.Abs((currentPos - transform.position).sqrMagnitude) > .1f)
            {
                Server.UpdateEntity(Entity.EntityId, FixedVector3.ComponentId, new PositionUpdate(Server, transform.position).Get().ToFixedVector3());
                updateTime = updateTick;
            }
        }
    }
}
