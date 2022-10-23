using Grpc.Core;
using MmoGameFramework;
using Mmogf.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Worlds
{

    public sealed class GridLayer
    {
        private ConcurrentDictionary<GridInt, List<int>> _cells = new ConcurrentDictionary<GridInt, List<int>>();
        private ConcurrentDictionary<int, GridInt> _entityCells = new ConcurrentDictionary<int, GridInt>();
        private ConcurrentDictionary<PositionInt, List<long>> _workerSubscriptions = new ConcurrentDictionary<PositionInt, List<long>>();
        //indexed by position, not grid

        public event Action<int, List<long>> OnEntityAdd;
        public event Action<int, List<long>> OnEntityRemove;

        public ConcurrentDictionary<GridInt, List<int>> Cells => _cells;
        public int CellSize { get; private set; }
        public int Layer { get; private set; }

        public GridLayer(int cellSize, int layer)
        {
            CellSize = cellSize;
            Layer = layer;
        }

        PositionInt GridIndexToPosition(GridInt grid)
        {
            return new PositionInt(grid.X * CellSize, grid.Y * CellSize, grid.Z * CellSize);
        }

        public (GridInt grid, PositionInt position, List<int> entities) AddEntity(Entity entity)
        {
            var pos = entity.Position;
            var cell = GetCell(pos);

            if(!cell.entities.Contains(entity.EntityId))
            {
                var ents = cell.entities;
                ents.Add(entity.EntityId);
                //write back to _cells
                if(_cells.TryUpdate(cell.grid, ents, cell.entities))
                {
                    List<long> subs;
                    if(!_workerSubscriptions.TryGetValue(cell.position, out subs))
                    {
                        subs = new List<long>();
                    }
                    OnEntityAdd?.Invoke(entity.EntityId, subs);
                    _entityCells.TryAdd(entity.EntityId, cell.grid);

                }
            }

            return cell;
        }

        public List<int> RemoveEntity(Entity entity)
        {
            var id = entity.EntityId;
            GridInt grid;
            if (!_entityCells.TryGetValue(id, out grid))
                return null;
            var cellPos = GridIndexToPosition(grid);
            var cell = GetCell(new Position(cellPos.X, cellPos.Y, cellPos.Z));
            var ents = cell.entities;
            ents.Remove(entity.EntityId);
            //write back to _cells
            if (_cells.TryUpdate(cell.grid, ents, cell.entities))
            {
                List<long> subs;
                if (!_workerSubscriptions.TryGetValue(cell.position, out subs))
                {
                    subs = new List<long>();
                }
                OnEntityRemove?.Invoke(entity.EntityId, subs);
                _entityCells.TryRemove(entity.EntityId, out cell.grid);

            }
            return cell.entities;
        }

        public (GridInt grid, PositionInt position, List<int> entities) GetCell(Position position)
        {
            var cellHalf = CellSize / 2.0;

            var cellX = (int)((Math.Abs(position.X) + cellHalf) / CellSize) * (position.X > 0 ? 1 : -1);
            var cellY = (int)((Math.Abs(position.Y) + cellHalf) / CellSize) * (position.Y > 0 ? 1 : -1);
            var cellZ = (int)((Math.Abs(position.Z) + cellHalf) / CellSize) * (position.Z > 0 ? 1 : -1);
            //add mutex
            List<int> cell;
            var grid = new GridInt(cellX, cellY, cellZ);
            if (!_cells.TryGetValue(grid, out cell))
            {
                cell = new List<int>(100);
                _cells.TryAdd(grid, cell);
            }
            var pos = GridIndexToPosition(grid);
            return (grid, pos, cell);
        }

        public Dictionary<PositionInt, (GridInt grid, List<int> entities)> GetCellsInArea(Position position, float interestArea)
        {
            var radius = interestArea / 2.0f;
            var cells = new Dictionary<PositionInt, (GridInt grid, List<int> entities)>(10);

            //shift offset by half a cell
            var maxBoundX = position.X + radius;
            var minBoundX = position.X - radius;
            var maxBoundY = position.Y + radius;
            var minBoundY = position.Y - radius;
            var maxBoundZ = position.Z + radius;
            var minBoundZ = position.Z - radius;

            var maxCellX = (int)Math.Ceiling(maxBoundX / CellSize);
            var minCellX = (int)Math.Floor(minBoundX / CellSize);
            var maxCellY = (int)Math.Ceiling(maxBoundY / CellSize);
            var minCellY = (int)Math.Floor(minBoundY / CellSize);
            var maxCellZ = (int)Math.Ceiling(maxBoundZ / CellSize);
            var minCellZ = (int)Math.Floor(minBoundZ / CellSize);

            for(int cntX = minCellX; cntX <= maxCellX; cntX++)
            {
                for (int cntZ = minCellZ; cntZ <= maxCellZ; cntZ++)
                {
                    for (int cntY = minCellY; cntY <= maxCellY; cntY++)
                    {
                        var worldPos = new Position(cntX * CellSize, cntY * CellSize, cntZ * CellSize);
                        var cell = GetCell(worldPos);
                        cells.Add(cell.position, (cell.grid,cell.entities));
                    }
                }
            }
            return cells;
        }

        public (List<int> addEntityIds, List<int> removeEntityIds, List<PositionInt> addCells, List<PositionInt> removeCells) UpdateWorkerInterestArea(WorkerConnection worker)
        {
            List<int> addEntityIds = new List<int>();
            List<int> removeEntityIds = new List<int>();
            var addCells = new List<PositionInt>();
            var removeCells = new List<PositionInt>();
            var cells = GetCellsInArea(worker.InterestPosition, worker.InterestRange);

            foreach(var cell in cells)
            {
                if(AddWorkerSub(cell.Key, worker))
                {
                    addCells.Add(cell.Key);
                    addEntityIds.AddRange(cell.Value.entities);
                }
            }

            if(worker.CellSubs.ContainsKey(Layer))
            {
                var subs = worker.CellSubs[Layer];
                foreach (var sub in subs)
                {
                    if (!cells.ContainsKey(sub))
                    {
                        removeCells.Add(sub);
                        var cell = GetCell(new Position(sub.X, sub.Y, sub.Z));
                        removeEntityIds.AddRange(cell.entities);
                    }

                }
            }    
            
            for(int i = removeCells.Count - 1; i >= 0; i--)
            {
                var cell = removeCells[i];
                RemoveWorkerSub(cell, worker);
            }

            return (addEntityIds, removeEntityIds, addCells, removeCells);

        }

        public bool AddWorkerSub(PositionInt cellPos, WorkerConnection worker)
        {
            List<long> subs;
            if(!_workerSubscriptions.TryGetValue(cellPos, out subs))
            {
                subs = new List<long>();
                _workerSubscriptions.TryAdd(cellPos, subs);
            }
            if (!subs.Contains(worker.WorkerId))
            {
                subs.Add(worker.WorkerId);
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
            List<long> subs;
            if (!_workerSubscriptions.TryGetValue(cellPos, out subs))
            {
                subs = new List<long>();
                _workerSubscriptions.TryAdd(cellPos, subs);
            }
            if (subs.Contains(worker.WorkerId))
            {
                subs.Remove(worker.WorkerId);
                if (_workerSubscriptions.TryUpdate(cellPos, subs, _workerSubscriptions[cellPos]))
                {
                    worker.RemoveCellSubscription(Layer, cellPos);
                    return true;
                }
            }
            return false;
        }

        public List<long> GetWorkerSubscriptions(PositionInt cellPos)
        {
            if (_workerSubscriptions.TryGetValue(cellPos, out List<long> subs))
                return subs;

            return null;
        }
    }
}
