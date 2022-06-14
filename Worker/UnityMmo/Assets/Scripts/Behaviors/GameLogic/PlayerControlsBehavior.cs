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

            var forward = moveState.Forward * transform.forward;

            _rigidbody.velocity = (forward * _moveSpeed);
        }

    }
}
