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
        [SerializeField]
        UiManager _uiManager;

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

            _uiManager = Object.FindObjectOfType<UiManager>();
            if(_uiManager != null)
            {
                _uiManager.AttachPlayer(this);
            }

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

            var moveState = GetEntityComponent<MovementState>(MovementState.ComponentId).Value;

            var forward = Input.GetAxis("Vertical");
            var heading = Input.GetAxis("Horizontal");
            var health = (Health)Entity.Data[Health.ComponentId];
            if (health.Current <= 0)
            {
                forward = 0;
                heading = 0;
            }
            Forward = forward;
            var position = transform.position;

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

            var position3d = position.ToVector3d(Server);
            if (position3d != moveState.DesiredPosition)
            {
                moveState.DesiredPosition = position3d;
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
