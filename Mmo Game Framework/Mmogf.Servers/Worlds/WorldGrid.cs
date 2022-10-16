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


        public ConcurrentDictionary<(int x, int y, int z), WorldCell> Cells => _cells;
        public int CellSize { get; private set; }

        public WorldGrid(int cellSize)
        {
            CellSize = cellSize;
        }

        public WorldCell AddEntity(Entity entity)
        {
            var pos = entity.Position;
            var cell = GetCell(pos);
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
                cell = new WorldCell(new Position(cellX * CellSize, cellY * CellSize, cellZ * CellSize), CellSize);
                _cells.TryAdd((cellX, cellY, cellZ), cell);
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
                            cell = new WorldCell(new Position(cntX * CellSize, cntY * CellSize, cntZ * CellSize), CellSize);
                            if(!_cells.TryAdd((cntX, cntY, cntZ), cell))
                                _cells.TryGetValue((cntX, cntY, cntZ), out cell); //validate this for better concurrency
                        }
                        list.Add(cell);

                    }
                }
            }
            return list;
        }

        public void UpdateWorkerInterestArea(WorkerConnection worker)
        {
            var cells = GetCellsInArea(worker.InterestPosition, worker.InterestRange);

            foreach(var cell in cells)
            {
                cell.AddWorkerSub(worker);
            }

            for(int i = worker.CellSubscriptions.Count - 1; i >= 0; i--)
            {
                var cell = worker.CellSubscriptions[i];
                if (!cells.Contains(cell))
                {
                    cell.RemoveWorkerSub(worker);
                    worker.CellSubscriptions.RemoveAt(i);
                }
            }

        }


    }
}
