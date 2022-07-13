using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class WanderBehavior : BaseEntityBehavior
    {
        [SerializeField]
        float _turnSpeed = 120f;
        [SerializeField]
        float _moveSpeed = 5f;

        [SerializeField]
        [Range(-1, 1)]
        float _turnPower = 0;

        [SerializeField]
        float _turnTimer = 3f;

        [SerializeField]
        Rigidbody _rigidbody;

        bool _alive = true;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if(Entity.Data.ContainsKey(Health.ComponentId))
            {
                var health = (Health)Entity.Data[Health.ComponentId];
                _alive = health.Current > 0;
            }

            if(!_alive)
                return;

            _turnTimer -= Time.deltaTime;

            if(_turnTimer <= 0)
            {
                _turnTimer = Random.Range(10f,20f);
                _turnPower = Random.Range(-0.2f, 0.2f);
            }

            transform.Rotate(0, _turnSpeed * _turnPower * Time.deltaTime, 0, Space.Self);

            //transform.Translate(new Vector3(0, 0, _moveSpeed * Time.deltaTime), Space.Self);
        }

        private void FixedUpdate()
        {
            var move = _moveSpeed;

            if (!_alive)
                move = 0;
            //if(_rigidbody.velocity.magnitude > _moveSpeed)
            //    return;
            _rigidbody.velocity = (transform.forward * move);

        }
    }
}
