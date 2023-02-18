using MessagePack;
using Microsoft.Extensions.Logging;
using Mmogf.Core;
using Mmogf.Servers.Storage;
using Mmogf.Servers.Worlds;
using Prometheus;
using ServiceStack.Caching;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MmoGameFramework
{
    public sealed class EntityStore
    {
        private int lastId = 0;
        private readonly Gauge EntitiesGauge = Metrics.CreateGauge($"dragongf_entities", "Number of entities in the world.");

        IStorageService _storageService;

        private List<GridLayer> GridLayers = new List<GridLayer>(2);

        ILogger _logger;

        public event Action<Entity> OnCreateEntity;
        public event Action<CommandRequest> OnEntityCommand;
        public event Action<CommandResponse> OnEntityCommandResponse;
        public event Action<EntityUpdate, long> OnUpdateEntityPartial;
        public event Action<EventRequest> OnEntityEvent;
        public event Action<Entity> OnEntityDelete;

        public event Action<int, List<long>> OnEntityAddSubscription;
        public event Action<int, List<long>> OnEntityRemoveSubscription;

        public EntityStore(ILogger<EntityStore> logger, IStorageService storageService, int cellSize)
        {
            _logger = logger;
            _storageService = storageService;

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

        private void ProcessOnEntityAdd(int entityId, List<long> workers)
        {
            OnEntityAddSubscription?.Invoke(entityId, workers);
        }

        private void ProcessOnEntityRemove(int entityId, List<long> workers)
        {
            OnEntityRemoveSubscription?.Invoke(entityId, workers);
        }


        public Entity Create(string entityType, Position position, Rotation rotation, List<Acl> acls, int? entityId = null, Dictionary<short, byte[]> additionalData = null)
        {
            if(entityId.HasValue)
            {
                if(lastId <= entityId.Value)
                    lastId = entityId.Value + 1; 
            }
            else
            {
                entityId = ++lastId;
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

            AddEntity(entity); //new 

            //make this configurable in the future
            var gridIndex = 0;
            if(data.ContainsKey(PlayerCreator.ComponentId))
            {
                gridIndex = 1;
            }

            GridLayers[gridIndex].AddEntity(entity);
            //check if in other layers based on components

            OnCreateEntity?.Invoke(entity);

            return entity;
        }

        void AddEntity(Entity entity)
        {
            _storageService.Set($"Ent:{entity.EntityId}", entity.EntityId);
            _storageService.Set($"Ent:{entity.EntityId}:Comps", string.Join(",",entity.EntityData.Keys));
            foreach (var comp in entity.EntityData)
            {
                _storageService.Set($"Ent:{entity.EntityId}:{comp.Key}", comp.Value);
            }

            var entCount = _storageService.Increment("Ent:Count", 1);
            EntitiesGauge.Set(entCount);

        }

        void RemoveEntity(int entityId)
        {
            _storageService.Remove($"Ent:{entityId}");
            var compIdKey = $"Ent:{entityId}:Comps";
            var comps = _storageService.Get<string>(compIdKey);
            if(comps == null)
                return;
            var compList = comps.Split(",");
            _storageService.Remove(compIdKey);
            foreach (var compId in compList)
            {
                _storageService.Remove($"Ent:{entityId}:{compId}");
            }

            var entCount = _storageService.Decrement("Ent:Count", 1);
            EntitiesGauge.Set(entCount);

        }

        public Entity? GetEntity(int entityId)
        {

            //how do we create this entity info?
            var compIdKey = $"Ent:{entityId}:Comps";
            var comps = _storageService.Get<string>(compIdKey);
            if (comps == null)
                return null;
            var compList = comps.Split(",");
            var data = new Dictionary<short, byte[]>();
            foreach (var compId in compList)
            {
                short compIdShort;
                if (!short.TryParse(compId, out compIdShort))
                    continue;
                var info = _storageService.Get<byte[]>($"Ent:{entityId}:{compId}");
                data.Add(compIdShort, info);
            }

            var entityInfo = new Entity(entityId, data);

            return entityInfo;        
        }

        public void UpdateEntityPartial(Entity entity, EntityUpdate entityUpdate, long workerId)
        {
            _storageService.Set($"Ent:{entityUpdate.EntityId}:{entityUpdate.ComponentId}", entityUpdate.Info);

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

        public void Delete(int entityId)
        {
            var entity = GetEntity(entityId);
            if(entity == null)
                return;

            foreach(var layer in GridLayers)
            {
                layer.RemoveEntity(entity.Value);
            }

            RemoveEntity(entityId);

            OnEntityDelete?.Invoke(entity.Value);

        }

        public (List<int> addEntityIds, List<int> removeEntityIds) UpdateWorkerInterestArea(WorkerConnection worker)
        {
            List<int> addEntityIds = new List<int>();
            List<int> removeEntityIds = new List<int>();
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
