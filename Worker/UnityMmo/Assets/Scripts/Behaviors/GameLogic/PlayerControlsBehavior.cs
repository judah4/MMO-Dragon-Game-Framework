using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class PlayerControlsBehavior : BaseEntityBehavior
    {
        [SerializeField]
        float _turnSpeed = 120f;
        [SerializeField]
        float _moveSpeed = 5f;

        [SerializeField]
        Rigidbody _rigidbody;

        float _desiredDistanceThreshold = 1.0f;
        bool _restrictVertical = true;

        // Update is called once per frame
        void Update()
        {
            var moveState = (MovementState)Entity.Data[MovementState.ComponentId];

            var heading = moveState.Heading * _turnSpeed;

            transform.Rotate(xAngle: 0, heading * Time.deltaTime, 0, Space.Self);

            //transform.Translate(new Vector3(0, 0, forward * Time.deltaTime), Space.Self);
        }

        private void FixedUpdate()
        {

            var moveState = (MovementState)Entity.Data[MovementState.ComponentId];
            var health = (Health)Entity.Data[Health.ComponentId];

            var desiredPosition = moveState.DesiredPosition.ToVector3(Server);

            var forward = moveState.Forward * transform.forward;

            if(health.Current <= 0)
            {
                forward = Vector3.zero;
            }
            
            var desiredDirection = desiredPosition - transform.position;
            if(_restrictVertical)
                desiredDirection.y = 0;

            //check tolerance variable
            if (desiredDirection.sqrMagnitude > _desiredDistanceThreshold * _desiredDistanceThreshold)
            {
                desiredDirection.Normalize();

                forward += desiredDirection;
                forward.Normalize();
                forward.y = 0;
            }

            _rigidbody.velocity = (forward * _moveSpeed);
        }

    }
}
