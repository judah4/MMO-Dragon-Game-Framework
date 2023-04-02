using MessagePack;
using Microsoft.Extensions.Logging;
using Mmogf.Core;
using Mmogf.Servers.Worlds;
using Prometheus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MmoGameFramework
{
    public sealed class EntityStore
    {
        private int lastId = 0;
        private readonly Gauge EntitiesGauge = Metrics.CreateGauge($"dragongf_entities", "Number of entities in the world.");

        private ConcurrentDictionary<EntityId, Entity> _entities = new ConcurrentDictionary<EntityId, Entity>();
        
        private List<GridLayer> GridLayers = new List<GridLayer>(2);

        ILogger _logger;

        public ConcurrentDictionary<EntityId, Entity> Entities => _entities;

        public event Action<Entity> OnUpdateEntityFull;
        public event Action<CommandRequest> OnEntityCommand;
        public event Action<CommandResponse> OnEntityCommandResponse;
        public event Action<EntityUpdate, long> OnUpdateEntityPartial;
        public event Action<EventRequest> OnEntityEvent;
        public event Action<Entity> OnEntityDelete;

        public event Action<EntityId, List<long>> OnEntityAddSubscription;
        public event Action<EntityId, List<long>> OnEntityRemoveSubscription;

        public EntityStore(ILogger<EntityStore> logger, int cellSize)
        {
            _logger = logger;

            var grid1 = new GridLayer(cellSize, 0);
            //default 2 layers. regular checkout and infinite size
            var grid2 = new GridLayer(1000000, 1);
            AddGrid(grid1); //make sure we set the right layer indexes later
            AddGrid(grid2);

        }

        void AddGrid(GridLayer grid)
        {
            grid.OnEntityAdd += ProcessOnEntityAdd;
            grid.OnEntityRemove += ProcessOnEntityRemove;
            GridLayers.Add(grid);
        }

        private void ProcessOnEntityAdd(EntityId entityId, List<long> workers)
        {
            OnEntityAddSubscription?.Invoke(entityId, workers);
        }

        private void ProcessOnEntityRemove(EntityId entityId, List<long> workers)
        {
            OnEntityRemoveSubscription?.Invoke(entityId, workers);
        }


        public Entity Create(string entityType, Position position, Rotation rotation, List<Acl> acls, EntityId? entityId = null, Dictionary<short, byte[]> additionalData = null)
        {
            if(entityId.HasValue)
            {
                if(lastId <= entityId.Value.Id)
                    lastId = entityId.Value.Id + 1; 
            }
            else
            {
                entityId = new EntityId(++lastId);
            }

            //todo: Validate acl list for data passsed
            var data = new Dictionary<short, byte[]>()
            {
                { EntityType.ComponentId, MessagePackSerializer.Serialize(new EntityType() { Name = entityType}) },
                { FixedVector3.ComponentId, MessagePackSerializer.Serialize(position.ToFixedVector3()) },
                { Rotation.ComponentId, MessagePackSerializer.Serialize(rotation) },
                { Acls.ComponentId, MessagePackSerializer.Serialize(new Acls() { AclList = acls }) },
            };
            if (additionalData != null)
            {
                foreach (var additional in additionalData)
                {
                    if (data.ContainsKey(additional.Key))
                        continue;

                    data.Add(additional.Key, additional.Value);
                }
            }

            var entity = new Entity(entityId.Value, data);

            _entities.TryAdd(entityId.Value, entity);

            //make this configurable in the future
            var gridIndex = 0;
            if(data.ContainsKey(PlayerCreator.ComponentId))
            {
                gridIndex = 1;
            }

            GridLayers[gridIndex].AddEntity(entity);
            //check if in other layers based on components

            EntitiesGauge.Set(_entities.Count);

            return entity;
        }

        public Entity? GetEntity(EntityId entityId)
        {
            Entity entityInfo;
            if(!_entities.TryGetValue(entityId, out entityInfo))
                return null;

            return entityInfo;        
        }

        public void UpdateEntity(Entity entity)
        {
            _entities[entity.EntityId] = entity;
            OnUpdateEntityFull?.Invoke(entity);
        }

        public void UpdateEntityPartial(Entity entity, EntityUpdate entityUpdate, long workerId)
        {
            _entities[entity.EntityId] = entity;
            OnUpdateEntityPartial?.Invoke(entityUpdate, workerId);

            if(entityUpdate.ComponentId == FixedVector3.ComponentId)
            {
                //update regions
                //make this configurable, figure out how to check the components later
                int gridIndex = 0;
                if (entity.EntityData.ContainsKey(PlayerCreator.ComponentId))
                {
                    gridIndex = 1;
                }
                var layer = GridLayers[gridIndex];
                var cell = layer.GetCell(entity.Position);
                if(cell.entities.Contains(entity.EntityId))
                    return;

                layer.RemoveEntity(entity);
                cell = layer.AddEntity(entity);

                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug($"Layer {layer.Layer} Entity {entity.EntityId} moved to cell {cell.position}");
            }

        }

        public void SendCommand(CommandRequest commandRequest)
        {
            OnEntityCommand?.Invoke(commandRequest);
        }

        public void SendCommandResponse(CommandResponse commandResponse)
        {
            OnEntityCommandResponse?.Invoke(commandResponse);
        }

        public void SendEvent(EventRequest eventRequest)
        {
            OnEntityEvent?.Invoke(eventRequest);
        }

        public void Delete(EntityId entityId)
        {
            Entity entity;
            if(!_entities.Remove(entityId, out entity))
                return;

            foreach(var layer in GridLayers)
            {
                layer.RemoveEntity(entity);
            }

            OnEntityDelete?.Invoke(entity);

            EntitiesGauge.Set(_entities.Count);

        }

        public (List<EntityId> addEntityIds, List<EntityId> removeEntityIds) UpdateWorkerInterestArea(WorkerConnection worker)
        {
            var addEntityIds = new List<EntityId>();
            var removeEntityIds = new List<EntityId>();
            foreach (var layer in GridLayers)
            {
                var results = layer.UpdateWorkerInterestArea(worker);
                addEntityIds.AddRange(results.addEntityIds);
                removeEntityIds.AddRange(results.removeEntityIds);

                if (_logger.IsEnabled(LogLevel.Debug) && (results.addCells.Count > 0 || results.removeCells.Count > 0))
                    _logger.LogDebug($"({worker.InterestPosition.ToString()}) Layer {layer.Layer} Cells Added ({string.Join(',', results.addCells.Select(x=>x.ToString()))}), Cells Removed ({string.Join(',', results.removeCells.Select(x => x.ToString()))}) from Worker {worker.ConnectionType}-{worker.WorkerId}");


            }

            return (addEntityIds, removeEntityIds);
        }

        public void RemoveWorker(WorkerConnection worker)
        {
            var subs = worker.CellSubs;
            foreach(var sub in subs)
            {
                foreach(var layer in GridLayers)
                {
                    if(layer.Layer != sub.Key)
                        continue;

                    var subCells = sub.Value.Keys.ToList(); //figure out how to do this better
                    for (int cnt = subCells.Count - 1; cnt >= 0; cnt--)
                    {

                        layer.RemoveWorkerSub(subCells[cnt], worker);
                    }
                }
            }
            
        }

        public List<long> GetSubscribedWorkers(Entity entity)
        {
            var workerIds = new List<long>();
            //make this configurable, figure out how to check the components later
            int gridIndex = 0;
            if (entity.EntityData.ContainsKey(PlayerCreator.ComponentId))
            {
                gridIndex = 1;
            }
            var layer = GridLayers[gridIndex];
            var cell = layer.GetCell(entity.Position);
            
            var workerSubs = layer.GetWorkerSubscriptions(cell.position);

            foreach(var workerPair in workerSubs)
            {
                workerIds.Add(workerPair);
            }

            return workerIds;
        }
    }
}
