using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class Cannonball : MonoBehaviour
    {
        public Rigidbody Rigidbody => _rigidbody;
        [SerializeField]
        Rigidbody _rigidbody;

        [SerializeField]
        FireBehavior _fireBehavior;

        bool _destroy = false;
        float _destroyTimer = 0.25f;

        // Update is called once per frame
        void Update()
        {
            if(transform.position.y < -10)
                _destroy = true;

            if(_destroy)
            {
                _destroyTimer -= Time.deltaTime;
                if(_destroyTimer < 0)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            _destroy = true;
            if(_fireBehavior != null)
            {
                var healthBehavior = collision.collider.GetComponent<HealthBehavior>();

                if(healthBehavior != null)
                {
                    _fireBehavior.Server.SendCommand<Health.TakeDamageCommand, TakeDamageRequest, TakeDamageResponse>(healthBehavior.Entity.EntityId, Health.ComponentId, new TakeDamageRequest() { Amount = 10 }, result =>
                    {
                        if(result.CommandStatus != Core.CommandStatus.Success)
                        {
#if UNITY_EDITOR
                            Debug.LogError($"Take Damage: {result.CommandStatus}: {result.Message}");
#endif
                            return;
                        }
#if UNITY_EDITOR
                        Debug.Log($"Damage dealt! {result.Request?.Amount}, Dead:{result.Response?.Dead}, Killed:{result.Response?.Killed}");
#endif
                    });
                }
            }

        }

        internal void InitServer(FireBehavior fireBehavior)
        {
            _fireBehavior = fireBehavior;
        }
    }
}
