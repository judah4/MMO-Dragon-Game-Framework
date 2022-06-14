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

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnCollisionEnter(Collision collision)
        {
            #if UNITY_EDITOR
            Debug.Log($"Colliding with {collision.collider.name} Server:{(_fireBehavior != null)}");
            #endif
            if(_fireBehavior != null)
            {
                var healthBehavior = collision.collider.GetComponent<HealthBehavior>();

                if(healthBehavior != null)
                {
                    _fireBehavior.Server.SendCommand<Health.TakeDamageCommand, TakeDamageRequest, TakeDamageResponse>(healthBehavior.Entity.EntityId, Health.ComponentId, new TakeDamageRequest() { Amount = 10 }, result =>
                    {
                        if(result.CommandStatus != Core.CommandStatus.Success)
                        {
                            Debug.LogError($"Take Damage: {result.CommandStatus}: {result.Message}");
                            return;
                        }

                        Debug.Log($"Damage dealt! {result.Request?.Amount}, Dead:{result.Response?.Dead}, Killed:{result.Response?.Killed}");
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
