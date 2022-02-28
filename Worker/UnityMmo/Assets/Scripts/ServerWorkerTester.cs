using Mmogf;
using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class ServerWorkerTester : MonoBehaviour
    {
        [SerializeField]
        ServerHandler _serverHandler;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {

            //debugging
            if (Input.GetKeyDown(KeyCode.S))
            {
                //create ship
                _serverHandler.SendCommand(0, 0, new World.CreateEntity("Ship", new Position() { Y = 1, }, Rotation.Identity, 
                    new Dictionary<int, byte[]>() 
                    {
                        { Cannon.ComponentId, MessagePack.MessagePackSerializer.Serialize(new Cannon()) },
                    }, 
                    new List<Acl>() 
                    {
                        new Acl() { ComponentId = Position.ComponentId, WorkerType = "Dragon-Worker" },
                        new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                        new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
                        new Acl() { ComponentId = Cannon.ComponentId, WorkerType = "Dragon-Worker" },
                    }), 
                    response => {
                        Debug.Log($"Create Entity! {response.CommandStatus} - {response.Message}");
                    });
            }
        
        }
    }
}
