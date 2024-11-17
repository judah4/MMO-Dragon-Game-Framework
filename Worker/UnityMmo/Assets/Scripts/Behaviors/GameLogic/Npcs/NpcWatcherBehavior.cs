using Mmogf.Core;
using Mmogf.Core.Contracts;
using Mmogf.Servers.Shared;
using UnityEngine;

namespace Mmogf
{
    public class NpcWatcherBehavior : BaseEntityBehavior
    {
        [SerializeField]
        float _deadTimer = 0;
        [SerializeField]
        bool _sentDestroy = false;
        [SerializeField]
        float _maxRange = 500f;

        [SerializeField]
        UpdatePositionSender _postionSender;
        [SerializeField]
        string _playerEntityName = "Player";

        float _playerCheckTimer = 1f;


        // Update is called once per frame
        void Update()
        {

            if (!HasAuthority(FixedVector3.ComponentId))
                return;

            var pos = GetEntityComponent<FixedVector3>().Value.ToPosition();

            CheckForPlayers(pos);

            var health = GetEntityComponent<Health>();

            if (health.Value.Current < 1)
            {
                _deadTimer += Time.deltaTime;
                if (_deadTimer > 30)
                    SendDestroy();
            }


            if (System.Math.Abs(pos.X) > _maxRange)
                SendDestroy();
            if (System.Math.Abs(pos.Z) > _maxRange)
                SendDestroy();

        }

        void SendDestroy()
        {
            if (_sentDestroy)
                return;

            Server.SendWorldCommand<World.DeleteEntity, DeleteEntityRequest, NothingInternal>(new DeleteEntityRequest(Entity.EntityId), response =>
            {
                Debug.Log($"Deleted Npc! {response.CommandStatus} - {response.Message}");
            });

            _sentDestroy = true;
        }

        void CheckForPlayers(Position position)
        {
            _playerCheckTimer -= Time.deltaTime;

            if (_playerCheckTimer > 0)
                return;

            float playerDistance = 9999;

            foreach (var entityPair in Server.GameObjectRepresentation.Entities)
            {
                var entType = entityPair.Value.GetEntityComponent<EntityType>(EntityType.ComponentId);

                if (entType.Value.Name == _playerEntityName)
                {
                    var playerPos = entityPair.Value.GetEntityComponent<FixedVector3>(FixedVector3.ComponentId).Value.ToPosition();

                    var dist = (float)Position.Distance(position, playerPos);
                    if (dist < playerDistance)
                        playerDistance = dist;

                }

            }

            float tickRate = 1f;
            float rotationTickRate = 1f;
            bool reset = false;
            if (playerDistance > 50f) //50m+
            {
                tickRate = 1f;
            }
            else if (playerDistance > 30f) //30m-50m
            {
                tickRate = 0.5f;
                rotationTickRate = 0.5f;
            }
            else if (playerDistance > 20) //20m-30m
            {
                tickRate = 0.2f;
                rotationTickRate = .3f;
            }
            else if (playerDistance > 15f) //15m-20m
            {
                tickRate = 0.15f;
                rotationTickRate = .3f;
            }
            else
            {
                _postionSender.ResetUpdateRate();
                reset = true;
            }

            if (!reset)
            {
                _postionSender.SetUpdateRate(tickRate);
                _postionSender.SetRotationUpdateRate(rotationTickRate);
            }

            _playerCheckTimer = 10f;

        }
    }
}
