using MmoGameFramework;
using Mmogf.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Worlds
{
    public class WorldCell
    {
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
