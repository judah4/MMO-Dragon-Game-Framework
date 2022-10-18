using Grpc.Core;
using MmoGameFramework;
using Mmogf.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Worlds
{
    public sealed class WorldGrid
    {
        private ConcurrentDictionary<(int x, int y, int z), WorldCell> _cells = new ConcurrentDictionary<(int x, int y, int z), WorldCell>();
        private ConcurrentDictionary<int, WorldCell> _entityCells = new ConcurrentDictionary<int, WorldCell>();

        public event Action<int, ConcurrentDictionary<long, string>> OnEntityAdd;
        public event Action<int, ConcurrentDictionary<long, string>> OnEntityRemove;

        public ConcurrentDictionary<(int x, int y, int z), WorldCell> Cells => _cells;
        public int CellSize { get; private set; }
        public int Layer { get; private set; }

        public WorldGrid(int cellSize, int layer)
        {
            CellSize = cellSize;
            Layer = layer;
        }

        public WorldCell AddEntity(Entity entity, WorldCell cell = null)
        {
            var pos = entity.Position;
            if(cell == null)
                cell = GetCell(pos);
            cell.AddEntity(entity);
            _entityCells.TryAdd(entity.EntityId, cell);
            return cell;
        }

        public WorldCell RemoveEntity(Entity entity)
        {
            var id = entity.EntityId;
            WorldCell cell;
            if (!_entityCells.TryGetValue(id, out cell))
                return null;
            cell.RemoveEntity(entity);
            _entityCells.TryRemove(id, out cell);
            return cell;
        }

        public WorldCell GetCell(Position position)
        {
            var cellHalf = CellSize / 2.0;

            var cellX = (int)((position.X + cellHalf) / CellSize);
            var cellY = (int)((position.Y + cellHalf) / CellSize);
            var cellZ = (int)((position.Z + cellHalf) / CellSize);
            //add mutex
            WorldCell cell;
            if (!_cells.TryGetValue((cellX, cellY, cellZ), out cell))
            {
                cell = new WorldCell(new Position(cellX * CellSize, cellY * CellSize, cellZ * CellSize), CellSize, Layer);
                _cells.TryAdd((cellX, cellY, cellZ), cell);
                cell.OnEntityAdd += ProcessOnEntityAdd;
                cell.OnEntityRemove += ProcessOnEntityRemove;
            }
            return cell;
        }

        public List<WorldCell> GetCellsInArea(Position position, float interestArea)
        {
            var radius = interestArea / 2.0f;
            var list = new List<WorldCell>(10);

            var cellHalf = CellSize / 2.0;
            //shift offset by half a cell
            var maxBoundX = position.X + radius + cellHalf;
            var minBoundX = position.X - radius - cellHalf;
            var maxBoundY = position.Y + radius + cellHalf;
            var minBoundY = position.Y - radius - cellHalf;
            var maxBoundZ = position.Z + radius + cellHalf;
            var minBoundZ = position.Z - radius - cellHalf;

            var maxCellX = (int)Math.Floor(maxBoundX / CellSize);
            var minCellX = (int)Math.Ceiling(minBoundX / CellSize);
            var maxCellY = (int)Math.Floor(maxBoundY / CellSize);
            var minCellY = (int)Math.Ceiling(minBoundY / CellSize);
            var maxCellZ = (int)Math.Floor(maxBoundZ / CellSize);
            var minCellZ = (int)Math.Ceiling(minBoundZ / CellSize);

            for(int cntX = minCellX; cntX <= maxCellX; cntX++)
            {
                for (int cntZ = minCellZ; cntZ <= maxCellZ; cntZ++)
                {
                    for (int cntY = minCellY; cntY <= maxCellY; cntY++)
                    { 
                        WorldCell cell;
                        if(!_cells.TryGetValue((cntX,cntY,cntZ), out cell))
                        {
                            cell = new WorldCell(new Position(cntX * CellSize, cntY * CellSize, cntZ * CellSize), CellSize, Layer);
                            if(!_cells.TryAdd((cntX, cntY, cntZ), cell))
                                _cells.TryGetValue((cntX, cntY, cntZ), out cell); //validate this for better concurrency
                            else
                            {
                                cell.OnEntityAdd += ProcessOnEntityAdd;
                                cell.OnEntityRemove += ProcessOnEntityRemove;
                            }
                        }
                        list.Add(cell);

                    }
                }
            }
            return list;
        }

        public (List<int> addEntityIds, List<int> removeEntityIds, List<WorldCell> addCells, List<WorldCell> removeCells) UpdateWorkerInterestArea(WorkerConnection worker)
        {
            List<int> addEntityIds = new List<int>();
            List<int> removeEntityIds = new List<int>();
            var addCells = new List<WorldCell>();
            var removeCells = new List<WorldCell>();
            var cells = GetCellsInArea(worker.InterestPosition, worker.InterestRange);

            foreach(var cell in cells)
            {
                if(cell.AddWorkerSub(worker))
                {
                    addCells.Add(cell);
                    addEntityIds.AddRange(cell.Entities.Keys);
                }
            }

            for(int i = worker.CellSubscriptions.Count - 1; i >= 0; i--)
            {
                var cell = worker.CellSubscriptions[i];
                if(cell.Layer != Layer)
                    continue;
                if (!cells.Contains(cell))
                {
                    if(cell.RemoveWorkerSub(worker))
                    {
                        removeCells.Add(cell);
                        removeEntityIds.AddRange(cell.Entities.Keys);  
                    }
                }
            }

            return (addEntityIds, removeEntityIds, addCells, removeCells);

        }

        void ProcessOnEntityAdd(int entityId, ConcurrentDictionary<long, string> workers)
        {
            OnEntityAdd?.Invoke(entityId, workers);
        }
        void ProcessOnEntityRemove(int entityId, ConcurrentDictionary<long, string> workers)
        {
            OnEntityRemove?.Invoke(entityId, workers);
        }


    }
}
