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

        // Start is called before the first frame update
        void Start()
        {
            var clientAuthCheck = GetEntityComponent<ClientAuthCheck>(ClientAuthCheck.ComponentId);
            var hasAuth = clientAuthCheck.HasValue && clientAuthCheck.Value.WorkerId == Server.ClientId;
            Debug.Log($"Player auth: {hasAuth} - {Server.ClientId}");
            if(hasAuth)
            {
                CameraSystem.Instance.SetTarget(transform);
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

        void HandleMovement()
        {
            inputTimer -= Time.deltaTime;
            if (inputTimer > 0)
                return;

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

            if (hasUpdate)
            {
                Server.UpdateEntity(Entity.EntityId, MovementState.ComponentId, moveState);
                inputTimer = .10f;
            }
        }

        T? GetEntityComponent<T>(int componentId) where T : struct, IEntityComponent 
        {
            IEntityComponent component;
            if (Entity.Data.TryGetValue(componentId, out component))
            {
                return (T)component;
            }

            return null;

        }
    }
}
