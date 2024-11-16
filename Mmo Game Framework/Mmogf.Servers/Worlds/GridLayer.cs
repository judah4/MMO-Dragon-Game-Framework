using MmoGameFramework;
using Mmogf.Servers.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Mmogf.Servers.Worlds
{

    public sealed class GridLayer
    {
        private ConcurrentDictionary<GridInt, List<EntityId>> _cells = new ConcurrentDictionary<GridInt, List<EntityId>>();
        private ConcurrentDictionary<EntityId, GridInt> _entityCells = new ConcurrentDictionary<EntityId, GridInt>();
        private ConcurrentDictionary<PositionInt, ConcurrentDictionary<RemoteWorkerIdentifier, byte>> _workerSubscriptions = new ConcurrentDictionary<PositionInt, ConcurrentDictionary<RemoteWorkerIdentifier, byte>>();
        //indexed by position, not grid
        private object _lock = new object();

        public event Action<EntityId, IEnumerable<RemoteWorkerIdentifier>> OnEntityAdd;
        public event Action<EntityId, IEnumerable<RemoteWorkerIdentifier>> OnEntityRemove;

        public ConcurrentDictionary<GridInt, List<EntityId>> Cells => _cells;
        public int CellSize { get; private set; }
        public GridLayerIdentifier Layer { get; private set; }

        public GridLayer(int cellSize, GridLayerIdentifier layer)
        {
            CellSize = cellSize;
            Layer = layer;
        }

        PositionInt GridIndexToPosition(GridInt grid)
        {
            return new PositionInt(grid.X * CellSize, grid.Y * CellSize, grid.Z * CellSize);
        }

        public (GridInt grid, PositionInt position, List<EntityId> entities) AddEntity(Entity entity)
        {
            var pos = entity.Position;
            var cell = GetCell(pos);

            if (!cell.entities.Contains(entity.EntityId))
            {
                var ents = cell.entities;
                ents.Add(entity.EntityId);
                //write back to _cells
                if (_cells.TryUpdate(cell.grid, ents, cell.entities))
                {
                    ConcurrentDictionary<RemoteWorkerIdentifier, byte> subs;
                    if (!_workerSubscriptions.TryGetValue(cell.position, out subs))
                    {
                        subs = new ConcurrentDictionary<RemoteWorkerIdentifier, byte>();
                    }
                    OnEntityAdd?.Invoke(entity.EntityId, subs.Keys);
                    _entityCells.TryAdd(entity.EntityId, cell.grid);

                }
            }

            return cell;
        }

        public List<EntityId> RemoveEntity(Entity entity)
        {
            var id = entity.EntityId;
            GridInt grid;
            if (!_entityCells.TryGetValue(id, out grid))
                return null;
            var cellPos = GridIndexToPosition(grid);
            var cell = GetCell(new Position(cellPos.X, cellPos.Y, cellPos.Z));
            var ents = cell.entities; //todo: fix this list referecnce so we don't have to lock
            lock (_lock)
            {
                ents.Remove(entity.EntityId);
                //write back to _cells
                if (_cells.TryUpdate(cell.grid, ents, cell.entities))
                {
                    ConcurrentDictionary<RemoteWorkerIdentifier, byte> subs;
                    if (!_workerSubscriptions.TryGetValue(cell.position, out subs))
                    {
                        subs = new ConcurrentDictionary<RemoteWorkerIdentifier, byte>();
                    }
                    OnEntityRemove?.Invoke(entity.EntityId, subs.Keys);
                    _entityCells.TryRemove(entity.EntityId, out cell.grid);

                }
            }
            return cell.entities;
        }

        public (GridInt grid, PositionInt position, List<EntityId> entities) GetCell(Position position)
        {
            var cellHalf = CellSize / 2.0;

            var cellX = (int)((Math.Abs(position.X) + cellHalf) / CellSize) * (position.X > 0 ? 1 : -1);
            var cellY = (int)((Math.Abs(position.Y) + cellHalf) / CellSize) * (position.Y > 0 ? 1 : -1);
            var cellZ = (int)((Math.Abs(position.Z) + cellHalf) / CellSize) * (position.Z > 0 ? 1 : -1);
            //add mutex
            List<EntityId> cell;
            var grid = new GridInt(cellX, cellY, cellZ);
            if (!_cells.TryGetValue(grid, out cell))
            {
                cell = new List<EntityId>(100);
                _cells.TryAdd(grid, cell);
            }
            var pos = GridIndexToPosition(grid);
            return (grid, pos, cell);
        }

        public Dictionary<PositionInt, (GridInt grid, List<EntityId> entities)> GetCellsInArea(Position position, float interestArea)
        {
            var radius = interestArea / 2.0f;
            var cells = new Dictionary<PositionInt, (GridInt grid, List<EntityId> entities)>(10);

            //check cell half vs radius check out, whichever is bigger
            var checkoutBoundary = Math.Min(CellSize / 2.0f, radius);

            //shift offset by half a cell
            var maxBoundX = position.X + radius + checkoutBoundary;
            var minBoundX = position.X - radius - checkoutBoundary;
            var maxBoundY = position.Y + radius + checkoutBoundary;
            var minBoundY = position.Y - radius - checkoutBoundary;
            var maxBoundZ = position.Z + radius + checkoutBoundary;
            var minBoundZ = position.Z - radius - checkoutBoundary;

            var maxCellXd = Math.Round(maxBoundX / CellSize, MidpointRounding.AwayFromZero);
            var minCellXd = Math.Round(minBoundX / CellSize, MidpointRounding.AwayFromZero);
            var maxCellYd = Math.Round(maxBoundY / CellSize, MidpointRounding.AwayFromZero);
            var minCellYd = Math.Round(minBoundY / CellSize, MidpointRounding.AwayFromZero);
            var maxCellZd = Math.Round(maxBoundZ / CellSize, MidpointRounding.AwayFromZero);
            var minCellZd = Math.Round(minBoundZ / CellSize, MidpointRounding.AwayFromZero);

            var maxCellX = (int)maxCellXd; //Math.Ceiling(maxBoundX / CellSize);
            var minCellX = (int)minCellXd; //Math.Floor(minBoundX / CellSize);
            var maxCellY = (int)maxCellYd; //Math.Ceiling(maxBoundY / CellSize);
            var minCellY = (int)minCellYd; //Math.Floor(minBoundY / CellSize);
            var maxCellZ = (int)maxCellZd; //Math.Ceiling(maxBoundZ / CellSize);
            var minCellZ = (int)minCellZd; //Math.Floor(minBoundZ / CellSize);

            for (int cntX = minCellX; cntX <= maxCellX; cntX++)
            {
                for (int cntZ = minCellZ; cntZ <= maxCellZ; cntZ++)
                {
                    for (int cntY = minCellY; cntY <= maxCellY; cntY++)
                    {
                        var worldPos = new Position(cntX * CellSize, cntY * CellSize, cntZ * CellSize);
                        var cell = GetCell(worldPos);
                        cells.Add(cell.position, (cell.grid, cell.entities));
                    }
                }
            }
            return cells;
        }

        public (List<EntityId> addEntityIds, List<EntityId> removeEntityIds, List<PositionInt> addCells, List<PositionInt> removeCells) UpdateWorkerInterestArea(WorkerConnection worker)
        {
            List<EntityId> addEntityIds = new List<EntityId>();
            var removeEntityIds = new List<EntityId>();
            var addCells = new List<PositionInt>();
            var removeCells = new List<PositionInt>();
            var cells = GetCellsInArea(worker.InterestPosition, worker.InterestRange);

            foreach (var cell in cells)
            {
                if (AddWorkerSub(cell.Key, worker))
                {
                    addCells.Add(cell.Key);
                    addEntityIds.AddRange(cell.Value.entities);
                }
            }

            if (worker.CellSubs.ContainsKey(Layer))
            {
                var subs = worker.CellSubs[Layer];
                foreach (var sub in subs)
                {
                    if (!cells.ContainsKey(sub.Key))
                    {
                        removeCells.Add(sub.Key);
                        var cell = GetCell(new Position(sub.Key.X, sub.Key.Y, sub.Key.Z));
                        removeEntityIds.AddRange(cell.entities);
                    }

                }
            }

            for (int i = removeCells.Count - 1; i >= 0; i--)
            {
                var cell = removeCells[i];
                RemoveWorkerSub(cell, worker);
            }

            return (addEntityIds, removeEntityIds, addCells, removeCells);

        }

        public bool AddWorkerSub(PositionInt cellPos, WorkerConnection worker)
        {
            ConcurrentDictionary<RemoteWorkerIdentifier, byte> subs;
            if (!_workerSubscriptions.TryGetValue(cellPos, out subs))
            {
                subs = new ConcurrentDictionary<RemoteWorkerIdentifier, byte>();
                _workerSubscriptions.TryAdd(cellPos, subs);
            }
            if (!subs.ContainsKey(worker.WorkerId))
            {
                subs.TryAdd(worker.WorkerId, 1);
                if (_workerSubscriptions.TryUpdate(cellPos, subs, _workerSubscriptions[cellPos]))
                {
                    worker.AddCellSubscription(Layer, cellPos);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveWorkerSub(PositionInt cellPos, WorkerConnection worker)
        {
            ConcurrentDictionary<RemoteWorkerIdentifier, byte> subs;
            if (!_workerSubscriptions.TryGetValue(cellPos, out subs))
            {
                subs = new ConcurrentDictionary<RemoteWorkerIdentifier, byte>();
                _workerSubscriptions.TryAdd(cellPos, subs);
            }
            if (subs.ContainsKey(worker.WorkerId))
            {
                subs.Remove(worker.WorkerId, out _);
                if (_workerSubscriptions.TryUpdate(cellPos, subs, _workerSubscriptions[cellPos]))
                {
                    worker.RemoveCellSubscription(Layer, cellPos);
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<RemoteWorkerIdentifier> GetWorkerSubscriptions(PositionInt cellPos)
        {
            if (_workerSubscriptions.TryGetValue(cellPos, out ConcurrentDictionary<RemoteWorkerIdentifier, byte> subs))
                return subs.Keys;

            return null;
        }
    }
}
