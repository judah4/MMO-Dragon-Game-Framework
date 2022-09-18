using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class CannonFiring : MonoBehaviour
    {

        [SerializeField]
        private Cannonball _cannonballPrefab;

        [SerializeField]
        private float _fireSpreadRange;

        [SerializeField]
        [Range(1, 20)]
        private int _numberOfCannons = 1;
        [SerializeField]
        private int _fireNumber = 0;

        public Cannonball SpawnCannonball(bool left)
        {

            var offset = Vector3.right;
            if (left)
            {
                offset = Vector3.left;
            }

            var z = -_fireSpreadRange + ((_fireSpreadRange * 2) / _numberOfCannons * _fireNumber);

            offset.z += z;

            offset += Vector3.up;

            var point = transform.TransformPoint(offset);

            var cannonBall = Instantiate(_cannonballPrefab, point, transform.rotation);
            var velocity = Vector3.right * 20;
            if (left)
            {
                velocity *= -1;
            }
            var vel = transform.TransformDirection(velocity);
            cannonBall.Rigidbody.velocity = vel;

            _fireNumber = (_fireNumber + 1) % _numberOfCannons;
            return cannonBall;
        }
    }
}
