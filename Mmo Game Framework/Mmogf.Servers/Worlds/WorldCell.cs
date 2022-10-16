using MmoGameFramework;
using Mmogf.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Worlds
{
    /// <summary>
    /// Cells are from bottom left coordinates
    /// </summary>
    public class WorldCell
    {

        /*
                                (0,0,25)
                                | 
                                |
                                |
                                |
            (-25,0,0)--------(0,0,0)--------(25,0,0)
                                | 
                                |
                                |
                                |
                                (0,0,-25)
        */

        public int EntityCount => _entities.Count;

        public Position Position { get; private set; }
        public int CellSize { get; private set; }
        private ConcurrentDictionary<int,int> _entities = new ConcurrentDictionary<int,int>();
        public WorldCell(Position position, int cellSize)
        {
            this.Position = position;
            this.CellSize = cellSize;
        }

        public void AddEntity(Entity entity)
        {
            if(!_entities.ContainsKey(entity.EntityId))
                _entities.TryAdd(entity.EntityId, entity.EntityId);
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity.EntityId, out int value);
        }

        public bool WithinArea(Position point)
        {
            var halfCell = CellSize / 2.0;
            var minX = Position.X - halfCell;
            var maxX = Position.X + halfCell;
            var minY = Position.Y - halfCell;
            var maxY = Position.Y + halfCell;
            var minZ = Position.Z - halfCell;
            var maxZ = Position.Z + halfCell;
            if (point.X >= maxX || point.X < minX)
                return false;
            if (point.Y >= maxY || point.Y < minY)
                return false;
            if (point.Z >= maxZ || point.Z < minZ)
                return false;
            return true;

        }

    }
}
