﻿using System;
using System.Collections.Generic;
using System.Text;
using Mmogf.Core;
using Mmogf.Servers.Worlds;

namespace MmoGameFramework
{
    public sealed class WorkerConnection
    {
        public long WorkerId => Connection.RemoteUniqueIdentifier;
        public string ConnectionType { get; set; }
        public Position InterestPosition { get; set; }
        public float InterestRange { get; set; }
        public Lidgren.Network.NetConnection Connection { get; set; }

        public List<WorldCell> CellSubscriptions { get; set; }

        //figure out how to specify interest layers...
        /*
         World Layer
            Time, Weather, Synced Ocean Waves, etc
            Unlimited Bounds
        Normal Layer
            Players, Npcs, stuff
            100 meters or so
        Extended Range Layer
            Low update things like party markers, Town information
            1000 meters or so

         */

        public WorkerConnection(string connectionType, Lidgren.Network.NetConnection senderConnection, Position interestPosition)
        {
            ConnectionType = connectionType;
            InterestPosition = interestPosition;
            InterestRange = 100;
            Connection = senderConnection;
            CellSubscriptions = new List<WorldCell>();
        }

        public List<int> GetSubscribedEntityIds()
        {
            List<int> entities = new List<int>(100);
            foreach (var cell in CellSubscriptions)
            {
                foreach (var ent in cell.Entities)
                {
                    if(!entities.Contains(ent.Key))
                        entities.Add(ent.Key);
                }
            }

            return entities;
        }

    }
}
