using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{
    public class PlayerHeartbeatServerHandler : BaseEntityBehavior
    {
        public const float HeartbeatInterval = 5f;

        [SerializeField]
        float _heartbeatTimer = HeartbeatInterval;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            _heartbeatTimer -= Time.deltaTime;

            if(_heartbeatTimer < 0)
            {
                Debug.Log($"Player {Entity.EntityId} Sending Heartbeat Request.");

                Server.SendCommand(Entity.EntityId, PlayerHeartbeatClient.ComponentId, new PlayerHeartbeatClient.RequestHeartbeat(), response => {
                    var heartbeat = (PlayerHeartbeatServer)Entity.Data[PlayerHeartbeatServer.ComponentId];
                    Debug.Log($"Heartbeat {response.CommandStatus} - {response.Message}");
                    if(response.CommandStatus == CommandStatus.Success)
                    {
                        if(heartbeat.MissedHeartbeats > 0)
                        {
                            heartbeat.MissedHeartbeats = 0;
                            Server.UpdateEntity(Entity.EntityId, heartbeat.GetComponentId(), heartbeat);
                        }
                    }
                    else
                    {
                        heartbeat.MissedHeartbeats++;
                        Debug.Log($"Player {Entity.EntityId} missed Heartbeat {heartbeat.MissedHeartbeats}.");

                        if (heartbeat.MissedHeartbeats >= 3)
                        {
                            //Delete player
                            Debug.Log($"Deleting Player {Entity.EntityId} for missed Heartbeats.");
                            Server.SendCommand(0, 0, new World.DeleteEntity(Entity.EntityId));
                        }
                        else
                        {
                            Server.UpdateEntity(Entity.EntityId, heartbeat.GetComponentId(), heartbeat);
                        }

                    }
                    
                });

                _heartbeatTimer = HeartbeatInterval;
            }
        }
    }
}
