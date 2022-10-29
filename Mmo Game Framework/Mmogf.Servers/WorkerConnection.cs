using System;
using System.Collections.Concurrent;
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

        //the int value doesn't matter, we just want the concurrent set up.
        public ConcurrentDictionary<int, ConcurrentDictionary<PositionInt, int>> CellSubs { get; set; }
        /// <summary>
        /// Used for sending entity for when moving cells
        /// </summary>
        public ConcurrentDictionary<int,int> EntitiesToAdd { get; set; }
        /// <summary>
        /// Used for removing entity for when moving cells
        /// </summary>
        public ConcurrentDictionary<int, int> EntitiesToRemove { get; set; }

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

        public WorkerConnection(string connectionType, Lidgren.Network.NetConnection senderConnection, Position interestPosition, int interestRange)
        {
            if(interestRange < 1)
                interestRange = 10;

            ConnectionType = connectionType;
            InterestPosition = interestPosition;
            InterestRange = interestRange;
            Connection = senderConnection;
            EntitiesToAdd = new ConcurrentDictionary<int, int> ();
            EntitiesToRemove = new ConcurrentDictionary<int, int>();
            CellSubs = new ConcurrentDictionary<int, ConcurrentDictionary<PositionInt, int>>();
        }

        public void AddCellSubscription(int layer, PositionInt cellPos)
        {
            if(!CellSubs.ContainsKey(layer))
                CellSubs.TryAdd(layer, new ConcurrentDictionary<PositionInt, int>());

            var subs = CellSubs[layer];
            if(!subs.ContainsKey(cellPos))
                subs.TryAdd(cellPos, 0);
        }

        public void RemoveCellSubscription(int layer, PositionInt cellPos)
        {
            if (!CellSubs.ContainsKey(layer))
                CellSubs.TryAdd(layer, new ConcurrentDictionary<PositionInt, int>());

            var subs = CellSubs[layer];
            subs.TryRemove(cellPos, out int val);
        }

    }
}
