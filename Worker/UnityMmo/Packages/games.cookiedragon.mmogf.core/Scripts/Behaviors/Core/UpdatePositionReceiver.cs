using System.Collections;
using System.Collections.Generic;
using MessagePack;
using Mmogf.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mmogf.Core
{
    public class UpdatePositionReceiver : BaseEntityBehavior
    {
        [SerializeField]
        protected bool LocalControl;

        Position _targetPostion;
        Vector3 _lastVectorPostion;
        float _smoothTime = 0;
        Vector3 _targetVectorPostion;

        void OnEnable()
        {

            _targetPostion = GetEntityComponent<FixedVector3>().Value.ToPosition();
            _targetVectorPostion = Server.PositionToClient(_targetPostion);
            _lastVectorPostion = Server.PositionToClient(_targetPostion);

        }

        void OnDisable()
        {
        }

        void Update()
        {
            if(LocalControl)
                return;

            var position = GetEntityComponent<FixedVector3>().Value.ToPosition();
            var rotation = GetEntityComponent<Rotation>().Value;

            var updatedRot = rotation.ToQuaternion();

            if(_targetPostion != position)
            {
                _lastVectorPostion = transform.position;

                _targetPostion = position;
                _targetVectorPostion = Server.PositionToClient(_targetPostion);
                _smoothTime = 0;
            }

            _smoothTime += Time.deltaTime;
            transform.position = LinearPostion(_lastVectorPostion, _targetVectorPostion, _smoothTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, updatedRot, 1.2f * Time.deltaTime);

        }

        public static Vector3 LinearPostion(Vector3 pastPosition, Vector3 targetPosition, float deltaTime)
        {
            return Vector3.Lerp(pastPosition, targetPosition, deltaTime);
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
