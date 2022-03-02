using MessagePack;
using Mmogf;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireVisualizer : BaseEntityBehavior
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
