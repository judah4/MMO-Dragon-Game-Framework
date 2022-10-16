using MessagePack;
using Microsoft.Extensions.Logging;
using Mmogf.Core;
using Mmogf.Servers.Worlds;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static Grpc.Core.Metadata;

namespace MmoGameFramework
{
    public sealed class EntityStore
    {
        private int lastId = 0;

        private ConcurrentDictionary<int, Entity> _entities = new ConcurrentDictionary<int, Entity>();
        private List<WorldGrid> GridLayers = new List<WorldGrid>(2);

        ILogger _logger;

        public event Action<Entity> OnUpdateEntity;
        public event Action<CommandRequest> OnEntityCommand;
        public event Action<CommandResponse> OnEntityCommandResponse;
        public event Action<EntityUpdate, long> OnUpdateEntityPartial;
        public event Action<EventRequest> OnEntityEvent;
        public event Action<Entity> OnEntityDelete;

        public EntityStore(ILogger<EntityStore> logger)
        {
            _logger = logger;

            //get from config
            var grid1 = new WorldGrid(50);
            //default 2 layers. regular checkout and infinite size
            var grid2 = new WorldGrid(1000000);
            GridLayers.Add(grid1);
            GridLayers.Add(grid2);

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

            _entities.TryAdd(entityId.Value, entity);

            GridLayers[0].AddEntity(entity);
            //check if in other layers based on components

            return entity;
        }

        [Obsolete]
        public List<Entity> GetInArea(Position position, float radius)
        {
            var entities = new List<Entity>();
            foreach (var entityInfo in _entities)
            {
                if (Position.WithinArea(entityInfo.Value.Position, position, radius))
                {
                    entities.Add(entityInfo.Value);
                }
            }

            return entities;
        }

        public Entity? GetEntity(int entityId)
        {
            Entity entityInfo;
            if(!_entities.TryGetValue(entityId, out entityInfo))
                return null;

            return entityInfo;        
        }

        public void UpdateEntity(Entity entity)
        {
            _entities[entity.EntityId] = entity;
            OnUpdateEntity?.Invoke(entity);
        }

        public void UpdateEntityPartial(Entity entity, EntityUpdate entityUpdate, long workerId)
        {
            _entities[entity.EntityId] = entity;
            OnUpdateEntityPartial?.Invoke(entityUpdate, workerId);

            if(entityUpdate.ComponentId == FixedVector3.ComponentId)
            {
                //update regions
                var cell = GridLayers[0].GetCell(entity.Position);
                if(cell.Entities.ContainsKey(entity.EntityId))
                    return;

                GridLayers[0].RemoveEntity(entity);
                cell.AddEntity(entity);
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
            Entity entity;
            if(!_entities.Remove(entityId, out entity))
                return;

            foreach(var layer in GridLayers)
            {
                layer.RemoveEntity(entity);
            }

            OnEntityDelete?.Invoke(entity);
        }

        public void UpdateWorkerInterestArea(WorkerConnection worker)
        {
            foreach(var layer in GridLayers)
            {
                layer.UpdateWorkerInterestArea(worker);
            }
        }

        public void RemoveWorker(WorkerConnection worker)
        {
            for (int cnt = worker.CellSubscriptions.Count - 1; cnt >= 0; cnt--)
            {
                var cell = worker.CellSubscriptions[cnt];
                cell.RemoveWorkerSub(worker);
            }
        }

        public List<long> GetSubscribedWorkers(Entity entity)
        {
            var workerIds = new List<long>();
            //default to first layer, figure out how to check the components later

            var layer = GridLayers[0];
            var cell = layer.GetCell(entity.Position);

            foreach(var workerPair in cell.WorkerSubscriptions)
            {
                workerIds.Add(workerPair.Key);
            }

            return workerIds;
        }
    }
}
