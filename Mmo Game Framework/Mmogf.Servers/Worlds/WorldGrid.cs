using MmoGameFramework;
using Mmogf.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Worlds
{
    public class WorldGrid
    {
        private ConcurrentDictionary<(int x, int y, int z), WorldCell> _cells = new ConcurrentDictionary<(int x, int y, int z), WorldCell>();
        private ConcurrentDictionary<int, WorldCell> _entityCells = new ConcurrentDictionary<int, WorldCell>();

        public int CellSize { get; private set; }

        public WorldGrid(int cellSize)
        {
            CellSize = cellSize;
        }

        public void AddEntity(Entity entity)
        {
            var pos = entity.Position;
            var cell = GetCell(pos);
            cell.AddEntity(entity);
            _entityCells.TryAdd(entity.EntityId, cell);
        }

        public void RemoveEntity(Entity entity)
        {
            var id = entity.EntityId;
            WorldCell cell;
            if (!_entityCells.TryGetValue(id, out cell))
                return;
            cell.RemoveEntity(entity);
            _entityCells.TryRemove(id, out cell);
        }

        WorldCell GetCell(Position position)
        {
            var cellHalf = CellSize / 2.0;

            var cellX = (int)(position.X / CellSize + cellHalf);
            var cellY = (int)(position.Y / CellSize + cellHalf);
            var cellZ = (int)(position.Z / CellSize + cellHalf);
            //add mutex
            WorldCell cell;
            if (!_cells.TryGetValue((cellX, cellY, cellZ), out cell))
            {
                cell = new WorldCell(new Position(cellX, cellY, cellZ), CellSize);
                _cells.TryAdd((cellX, cellY, cellZ), cell);
            }
            return cell;
        }

        public List<WorldCell> GetCellsInArea(Position position, float interestArea)
        {
            var list = new List<WorldCell>(10);

            var cellHalf = CellSize / 2.0;
            //shift offset by half a cell
            var maxBoundX = position.X + interestArea + cellHalf;
            var minBoundX = position.X - interestArea + cellHalf;
            var maxBoundY = position.Y + interestArea + cellHalf;
            var minBoundY = position.Y - interestArea + cellHalf;
            var maxBoundZ = position.Z + interestArea + cellHalf;
            var minBoundZ = position.Z - interestArea + cellHalf;

            var maxCellX = (int)(maxBoundX / CellSize);
            var minCellX = (int)(minBoundX / CellSize);
            var maxCellY = (int)(maxBoundY / CellSize);
            var minCellY = (int)(minBoundY / CellSize);
            var maxCellZ = (int)(maxBoundZ / CellSize);
            var minCellZ = (int)(minBoundZ / CellSize);

            for(int cntX = minCellX; cntX < maxCellX; cntX++)
            {
                for (int cntZ = minCellZ; cntZ < maxCellZ; cntZ++)
                {
                    for (int cntY = minCellY; cntY < maxCellY; cntY++)
                    { 
                        WorldCell cell;
                        if(_cells.TryGetValue((cntX,cntY,cntZ), out cell))
                        {
                            list.Add(cell);
                        }
                    }
                }
            }

            return list;
        }


    }
}
