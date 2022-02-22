using Assets.Scripts.Helpers;
using Dragongf.Assets.MmogfMessages.GameLogic;
using MessagePack;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehavior : BaseEntityBehavior
{

    bool _left = false;

    void OnEnable()
    {
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Server.SendCommand(Entity.EntityId, Cannon.ComponentId, new Cannon.FireCommand() { Left = _left }, response => {
                Debug.Log("Fired Cannon!");
            });
            _left = !_left;
        }
    }
}
