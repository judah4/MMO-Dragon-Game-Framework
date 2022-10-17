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
    public sealed class WorldCell
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
        public ConcurrentDictionary<int, int> Entities => _entities;
        public int WorkerSubscriptionCount => _workerSubscriptions.Count;
        public ConcurrentDictionary<long, string> WorkerSubscriptions => _workerSubscriptions;
        public Position Position { get; private set; }
        public int CellSize { get; private set; }
        private ConcurrentDictionary<int,int> _entities = new ConcurrentDictionary<int,int>();
        private ConcurrentDictionary<long, string> _workerSubscriptions = new ConcurrentDictionary<long, string>();

        public event Action<int, ConcurrentDictionary<long, string>> OnEntityAdd;
        public event Action<int, ConcurrentDictionary<long, string>> OnEntityRemove;

        public WorldCell(Position position, int cellSize)
        {
            this.Position = position;
            this.CellSize = cellSize;
        }

        public void AddEntity(Entity entity)
        {
            if(!_entities.ContainsKey(entity.EntityId))
            {
                if(_entities.TryAdd(entity.EntityId, entity.EntityId))
                    OnEntityAdd?.Invoke(entity.EntityId, _workerSubscriptions);
            }
                
        }

        public void RemoveEntity(Entity entity)
        {
            if(_entities.Remove(entity.EntityId, out int value))
            {
                OnEntityRemove?.Invoke(entity.EntityId, _workerSubscriptions);
            }
        }

        public bool AddWorkerSub(WorkerConnection worker)
        {
            if (!_workerSubscriptions.ContainsKey(worker.WorkerId))
            {
                if (_workerSubscriptions.TryAdd(worker.WorkerId, worker.ConnectionType))
                {
                    worker.CellSubscriptions.Add(this);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveWorkerSub(WorkerConnection worker)
        {
            if(_workerSubscriptions.Remove(worker.WorkerId, out string value))
            {
                worker.CellSubscriptions.Remove(this);
                return true;
            }
            return false;
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
