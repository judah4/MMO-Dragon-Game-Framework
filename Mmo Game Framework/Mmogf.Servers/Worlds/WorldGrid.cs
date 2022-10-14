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
        private ConcurrentDictionary<(int x, int z), WorldCell> _cells = new ConcurrentDictionary<(int x, int z), WorldCell>();

        public void AddEntity(Entity entity)
        {
        }

        public void RemoveEntity(Entity entity)
        {
        }

        public List<WorldCell> GetCellsInArea(Position position, float interestArea)
        {

        }

        public bool WithinArea(Position point)
        {
            var minX = Position.X - CellSize/2f;
            var maxX = Position.X + CellSize / 2f;
            var minZ = Position.Z - CellSize / 2f;
            var maxZ = Position.Z + CellSize / 2f;
            if (point.X > maxX || point.X < minX)
                return false;
            if (point.Z > maxZ || point.Z < minZ)
                return false;
            return true;

        }

    }
}
