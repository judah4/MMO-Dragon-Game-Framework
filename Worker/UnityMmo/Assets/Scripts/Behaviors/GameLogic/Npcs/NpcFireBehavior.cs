using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class NpcFireBehavior : BaseEntityBehavior
    {
        [SerializeField]
        float _fireTimer = 1f;
        bool left = false;

        void OnEnable()
        {
        }

        void Update()
        {
            if(!HasAuthority(Cannon.ComponentId))
                return;

            _fireTimer -= Time.deltaTime;

            if(_fireTimer >= 0)
                return;

            _fireTimer = Random.Range(1f, 2f);

            Server.SendCommand<Cannon.FireCommand, FireCommandRequest, Nothing>(Entity.EntityId, Cannon.ComponentId, new FireCommandRequest() { Left = left }, result => {
                if (result.CommandStatus != CommandStatus.Success)
                {
                    Debug.LogError($"{result.CommandId}: {result.CommandStatus} - {result.Message}");
                    return;
                }
            });
            left = !left;
        }

    }
}
