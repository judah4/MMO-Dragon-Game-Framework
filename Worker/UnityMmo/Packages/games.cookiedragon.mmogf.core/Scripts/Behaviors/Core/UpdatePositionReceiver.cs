using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using MessagePack;
using Mmogf.Core;
using UnityEngine;
namespace Mmogf.Core
{
    public class UpdatePositionReceiver : BaseEntityBehavior
    {
        void OnEnable()
        {
            Entity.OnEntityUpdate += OnUpdate;

        }

        void OnDisable()
        {
            Entity.OnEntityUpdate -= OnUpdate;
        }

        void OnUpdate()
        {
            var position = MessagePackSerializer.Deserialize<Position>(Entity.Data[Position.ComponentId]);
            var rotation = MessagePackSerializer.Deserialize<Rotation>(Entity.Data[Rotation.ComponentId]);
            var localPos = Server.PositionToClient(position);

            var updatedRot = rotation.ToQuaternion();
            Debug.Log($"{Entity.EntityId} {position.ToString()} {localPos}, ROT {updatedRot}");

            transform.position = localPos;
            transform.rotation = updatedRot;


        }
    }
}
