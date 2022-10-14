using System.Collections;
using System.Collections.Generic;
using MessagePack;
using Mmogf.Core;
using UnityEngine;
namespace Mmogf.Core
{
    public class UpdatePositionReceiver : BaseEntityBehavior
    {
        [SerializeField]
        protected bool LocalControl;

        void OnEnable()
        {
            //Entity.OnEntityUpdate += OnUpdate;
        }

        void OnDisable()
        {
            //Entity.OnEntityUpdate -= OnUpdate;
        }

        void Update()
        {
            if(LocalControl)
                return;

            var position = GetEntityComponent<FixedVector3>().Value.ToPosition();
            var rotation = GetEntityComponent<Rotation>().Value;
            var localPos = Server.PositionToClient(position);

            var updatedRot = rotation.ToQuaternion();

            transform.position = SmoothPosition(transform.position, localPos, 5, Time.deltaTime);
            transform.rotation = updatedRot;

        }

        public static Vector3 SmoothPosition(Vector3 pastPosition, Vector3 targetPosition, float speed, float deltaTime)
        {
            Vector3 pos;
            pos.x = Damp(pastPosition.x, targetPosition.x, speed, deltaTime);
            pos.y = Damp(pastPosition.y, targetPosition.y, speed, deltaTime);
            pos.z = Damp(pastPosition.z, targetPosition.z, speed, deltaTime);
            return pos;
        }

        //https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
        public static float Damp(float source, float target, float lambda, float dt)
        {
            return Mathf.Lerp(source, target, 1 - Mathf.Exp(-lambda * dt));
}
    }
}
