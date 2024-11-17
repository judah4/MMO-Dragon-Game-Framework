using Mmogf.Core;
using Mmogf.Core.Contracts.Commands;
using UnityEngine;

namespace Mmogf
{
    public class NpcFireBehavior : BaseEntityBehavior
    {
        [SerializeField]
        float _fireTimer = 1f;
        bool left = false;

        bool _alive = true;

        void OnEnable()
        {
        }

        void Update()
        {
            if (!HasAuthority(Cannon.ComponentId))
                return;

            if (Entity.Data.ContainsKey(Health.ComponentId))
            {
                var health = GetEntityComponent<Health>().Value;
                _alive = health.Current > 0;
            }

            if (!_alive)
                return;

            _fireTimer -= Time.deltaTime;

            if (_fireTimer >= 0)
                return;

            _fireTimer = Random.Range(2f, 3f);

            Server.SendCommand<Cannon.FireCommand, FireCommandRequest, Nothing>(Entity.EntityId, Cannon.ComponentId, new FireCommandRequest() { Left = left }, result =>
            {
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
