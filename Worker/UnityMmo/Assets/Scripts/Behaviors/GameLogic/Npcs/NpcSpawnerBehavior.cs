using Mmogf.Core;
using Mmogf.Core.Contracts;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class NpcSpawnerBehavior : BaseEntityBehavior
    {
        [SerializeField]
        float _timer = 3f;

        [SerializeField]
        float _sharkTimer = 5f;

        // Update is called once per frame
        void Update()
        {
            if (!HasAuthority(FixedVector3.ComponentId))
                return;

            _timer -= Time.deltaTime;
            _sharkTimer -= Time.deltaTime;

            CheckShip();
            CheckShark();

        }

        void CheckShip()
        {
            if (_timer > 0)
                return;

            SpawnShip();

            _timer = 30f;
        }

        void CheckShark()
        {
            if (_sharkTimer > 0)
                return;

            SpawnShark();

            _sharkTimer = 60f;
        }

        void SpawnShip()
        {
            var position = transform.position;
            position.y = 0;
            var pos = Server.PositionToServer(position);

            //create ship
            Server.SendWorldCommand<World.CreateEntity, CreateEntityRequest, CreateEntityResponse>(new CreateEntityRequest("Ship", pos.ToFixedVector3(), RandomHeading().ToRotation(),
                new Dictionary<short, byte[]>()
                {
                    { Cannon.ComponentId, Server.Serializer.Serialize(new Cannon()) },
                    { Health.ComponentId, Server.Serializer.Serialize(new Health() { Current = 100, Max = 100, }) },
                },
                new List<Acl>()
                {
                    new Acl() { ComponentId = FixedVector3.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Cannon.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Health.ComponentId, WorkerType = "Dragon-Worker" },
                }),
                response =>
                {
                    Debug.Log($"Create Entity! {response.CommandStatus} - {response.Message}");
                });
        }

        void SpawnShark()
        {
            var position = transform.position;
            position.y = -5;
            var pos = Server.PositionToServer(position);
            //create shark
            Server.SendWorldCommand<World.CreateEntity, CreateEntityRequest, CreateEntityResponse>(new CreateEntityRequest("Shark", pos.ToFixedVector3(), RandomHeading().ToRotation(),
                new Dictionary<short, byte[]>()
                {
                    { Cannon.ComponentId, Server.Serializer.Serialize(new Cannon()) },
                },
                new List<Acl>()
                {
                    new Acl() { ComponentId = FixedVector3.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
                }),
                response =>
                {
                    Debug.Log($"Create Entity! {response.CommandStatus} - {response.Message}");
                });

        }

        Quaternion RandomHeading()
        {
            return Quaternion.Euler(0, Random.Range(-180, 180), 0);
        }
    }
}
