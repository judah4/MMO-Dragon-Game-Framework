using Mmogf.Core.Contracts;
using Mmogf.Servers;
using Mmogf.Servers.ServerInterfaces;
using Mmogf.Servers.Shared;
using Mmogf.Servers.Worlds;
using System.Collections.Generic;
using System.Linq;

namespace MmoGameFramework
{
    public sealed class EntityGrids
    {
        private readonly List<GridLayer> GridLayers = new List<GridLayer>(2);

        private readonly IEntityStore _entityStore;

        public EntityGrids(IEntityStore entityStore, int cellSize)
        {
            _entityStore = entityStore;
            var grid1 = new GridLayer(cellSize, new GridLayerIdentifier(0));
            //default 2 layers. regular checkout and infinite size
            var grid2 = new GridLayer(1000000, new GridLayerIdentifier(1));
            AddGrid(grid1); //make sure we set the right layer indexes later
            AddGrid(grid2);

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
            }

            return (addEntityIds, removeEntityIds);
        }

        public void RemoveWorker(WorkerConnection worker)
        {
            var subs = worker.CellSubs;
            foreach (var sub in subs)
            {
                foreach (var layer in GridLayers)
                {
                    if (layer.Layer != sub.Key)
                        continue;

                    var subCells = sub.Value.Keys.ToList(); //figure out how to do this better
                    for (int cnt = subCells.Count - 1; cnt >= 0; cnt--)
                    {

                        layer.RemoveWorkerSub(subCells[cnt], worker);
                    }
                }
            }

        }

        public List<RemoteWorkerIdentifier> GetSubscribedWorkers(Entity entity)
        {
            var workerIds = new List<RemoteWorkerIdentifier>();
            //make this configurable, figure out how to check the components later
            int gridIndex = 0;
            if (entity.AdditionalData.ContainsKey(PlayerCreator.ComponentId))
            {
                gridIndex = 1;
            }
            var layer = GridLayers[gridIndex];
            var cell = layer.GetCell(entity.Position);

            var workerSubs = layer.GetWorkerSubscriptions(cell.position);

            foreach (var workerPair in workerSubs)
            {
                workerIds.Add(workerPair);
            }

            return workerIds;
        }

        private void AddGrid(GridLayer grid)
        {
            GridLayers.Add(grid);
        }
    }
}
