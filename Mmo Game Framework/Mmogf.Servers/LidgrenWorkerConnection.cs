using Mmogf.Servers;
using Mmogf.Servers.Shared;
using Mmogf.Servers.Worlds;
using System;
using System.Collections.Concurrent;

namespace MmoGameFramework
{
    public sealed class LidgrenWorkerConnection : IWorkerConnection
    {
        public RemoteWorkerIdentifier WorkerId => new RemoteWorkerIdentifier(Connection.RemoteUniqueIdentifier);
        public string ConnectionType { get; set; }
        public Position InterestPosition { get; set; }
        public float InterestRange { get; set; }
        public Lidgren.Network.NetConnection Connection { get; set; }

        /// <summary>
        /// Key is layer, value is cell position.
        /// </summary>
        /// <remarks>
        /// The int value doesn't matter, we just want the concurrent set up.
        /// </remarks>
        public ConcurrentDictionary<GridLayerIdentifier, ConcurrentDictionary<PositionInt, int>> CellSubs { get; set; }
        /// <summary>
        /// Used for sending entity for when moving cells
        /// </summary>
        public ConcurrentDictionary<EntityId, EntityId> EntitiesToAdd { get; set; }
        /// <summary>
        /// Used for removing entity for when moving cells
        /// </summary>
        public ConcurrentDictionary<EntityId, EntityId> EntitiesToRemove { get; set; }

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

        public LidgrenWorkerConnection(string connectionType, Lidgren.Network.NetConnection senderConnection, Position interestPosition, int interestRange)
        {
            if (interestRange < 1)
            {
                throw new ArgumentException($"Interest range needs to be greater than 1.", nameof(interestRange));
            }

            ConnectionType = connectionType;
            InterestPosition = interestPosition;
            InterestRange = interestRange;
            Connection = senderConnection;
            EntitiesToAdd = new ConcurrentDictionary<EntityId, EntityId>();
            EntitiesToRemove = new ConcurrentDictionary<EntityId, EntityId>();
            CellSubs = new ConcurrentDictionary<GridLayerIdentifier, ConcurrentDictionary<PositionInt, int>>();
        }

        public void AddCellSubscription(GridLayerIdentifier layer, PositionInt cellPos)
        {
            if (!CellSubs.ContainsKey(layer))
                CellSubs.TryAdd(layer, new ConcurrentDictionary<PositionInt, int>());

            var subs = CellSubs[layer];
            if (!subs.ContainsKey(cellPos))
                subs.TryAdd(cellPos, 0);
        }

        public void RemoveCellSubscription(GridLayerIdentifier layer, PositionInt cellPos)
        {
            if (!CellSubs.ContainsKey(layer))
                CellSubs.TryAdd(layer, new ConcurrentDictionary<PositionInt, int>());

            var subs = CellSubs[layer];
            subs.TryRemove(cellPos, out int val);
        }

    }
}
