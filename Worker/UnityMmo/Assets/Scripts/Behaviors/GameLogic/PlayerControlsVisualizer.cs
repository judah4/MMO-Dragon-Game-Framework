using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class PlayerControlsVisualizer : BaseEntityBehavior
    {
        public float inputTimer = -1;

        [Range(-1, 1)]
        public float Forward;

        [SerializeField]
        float _turnSpeed = 120f;
        [SerializeField]
        float _moveSpeed = 5f;

        [SerializeField]
        Rigidbody _rigidbody;

        bool inited = false;

        // Start is called before the first frame update
        void OnEnable()
        {
            var clientAuthCheck = GetEntityComponent<ClientAuthCheck>(ClientAuthCheck.ComponentId);
            var hasAuth = clientAuthCheck.HasValue && clientAuthCheck.Value.WorkerId == Server.ClientId;
            Debug.Log($"Player auth: {hasAuth} - AuthId:{clientAuthCheck.Value.WorkerId} ServerId:{Server.ClientId} - Type:{Server.WorkerType}");
            if(hasAuth)
            {
                Init();
            }

            Entity.OnEntityUpdate += Entity_OnEntityUpdate;

        }

        // Update is called once per frame
        void Update()
        {
            var clientAuthCheck = GetEntityComponent<ClientAuthCheck>(ClientAuthCheck.ComponentId);
            var hasAuth = clientAuthCheck.HasValue && clientAuthCheck.Value.WorkerId == Server.ClientId;

            if(!hasAuth)
                return;

            HandleMovement();
        }

        void Init()
        {
            CameraSystem.Instance.SetTarget(transform);
            inited = true;
        }

        void HandleMovement()
        {
            inputTimer -= Time.deltaTime;

            var moveState = (MovementState)Entity.Data[MovementState.ComponentId];

            var forward = Input.GetAxis("Vertical");
            var heading = Input.GetAxis("Horizontal");
            Forward = forward;

            bool hasUpdate = false;
            if(forward != moveState.Forward)
            {
                moveState.Forward = forward;
                hasUpdate = true;
            }
            if(heading != moveState.Heading)
            {
                moveState.Heading = heading;
                hasUpdate = true;
            }

            var headingMove = heading * _turnSpeed;

            //local move
            transform.Rotate(0, headingMove * Time.deltaTime, 0, Space.Self);
            //transform.Translate(new Vector3(0, 0, forwardMove * Time.deltaTime), Space.Self);

            if (hasUpdate && inputTimer <= 0)
            {
                Server.UpdateEntity(Entity.EntityId, MovementState.ComponentId, moveState);
                inputTimer = .10f;
            }
        }

        private void FixedUpdate()
        {
            var forwardMove = Forward * _moveSpeed;

            _rigidbody.velocity = (transform.forward * forwardMove);

        }

        private void Entity_OnEntityUpdate()
        {
            if(inited)
                return;

            var clientAuthCheck = GetEntityComponent<ClientAuthCheck>(ClientAuthCheck.ComponentId);
            var hasAuth = clientAuthCheck.HasValue && clientAuthCheck.Value.WorkerId == Server.ClientId;
            if (hasAuth)
            {
                Init();
            }
        }

    }
}
