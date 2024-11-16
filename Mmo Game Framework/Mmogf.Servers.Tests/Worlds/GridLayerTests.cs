using MmoGameFramework;
using Mmogf.Servers.Contracts;
using Mmogf.Servers.Shared;
using Mmogf.Servers.Worlds;

namespace Mmogf.Servers.Tests.Worlds
{
    [TestClass]
    public class GridLayerTests
    {
        GridLayer GetWorldGridWithDefaultLayer(int cellSize, bool addEntity)
        {
            return GetWorldGrid(cellSize, addEntity, new GridLayerIdentifier(0));
        }

        GridLayer GetWorldGrid(int cellSize, bool addEntity, GridLayerIdentifier layer)
        {
            var grid = new GridLayer(cellSize, layer);

            if (addEntity)
            {
                grid.AddEntity(GetEntity(new Position(1, 1, 1)));
            }

            return grid;
        }

        Entity GetEntity(Position postion)
        {
            var entity = new Entity(new EntityId(1), new Dictionary<short, byte[]>()
            {
                { EntityType.ComponentId, MessagePack.MessagePackSerializer.Serialize(new EntityType() { Name = "Npc" }) },
                { FixedVector3.ComponentId, MessagePack.MessagePackSerializer.Serialize<FixedVector3>(postion.ToFixedVector3()) },
                { Rotation.ComponentId, MessagePack.MessagePackSerializer.Serialize(Rotation.Zero) },
                { Acls.ComponentId, MessagePack.MessagePackSerializer.Serialize(new Acls() { AclList = new List<Acl>(), }) },
            });
            return entity;
        }

        [TestMethod]
        public void GetCell_Test()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            var cell = grid.GetCell(new Position(1, 1, 1));
            Assert.IsNotNull(cell);
            Assert.AreEqual(new PositionInt(0, 0, 0), cell.position);
            Assert.AreEqual(1, grid.Cells.Count);
            Assert.IsTrue(grid.Cells.TryGetValue(new GridInt(0, 0, 0), out List<EntityId>? c));
        }

        [TestMethod]
        public void GetCell_Test2()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            var cell = grid.GetCell(new Position(25, 1, 25));
            Assert.IsNotNull(cell);
            Assert.AreEqual(new GridInt(1, 0, 1), cell.grid);
            Assert.AreEqual(new PositionInt(50, 0, 50), cell.position);
            Assert.AreEqual(1, grid.Cells.Count);
            Assert.IsTrue(grid.Cells.TryGetValue(new GridInt(1, 0, 1), out List<EntityId>? c));
        }

        [TestMethod]
        public void GetCell_Test3()
        {
            var cellSize = 25;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            var cell = grid.GetCell(new Position(15, 1, 15));
            Assert.IsNotNull(cell);
            Assert.AreEqual(new GridInt(1, 0, 1), cell.grid);
            Assert.AreEqual(new PositionInt(25, 0, 25), cell.position);
            Assert.AreEqual(1, grid.Cells.Count);
            Assert.IsTrue(grid.Cells.TryGetValue(new GridInt(1, 0, 1), out List<EntityId>? c));
        }

        [TestMethod]
        public void GetCell_Test4()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            var cell = grid.GetCell(new Position(-30, -50, -30));
            Assert.IsNotNull(cell);
            Assert.AreEqual(new GridInt(-1, -1, -1), cell.grid);
            Assert.AreEqual(new PositionInt(-50, -50, -50), cell.position);
            Assert.AreEqual(1, grid.Cells.Count);
        }

        [TestMethod]
        public void GetCell_Test5()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            var cell = grid.GetCell(new Position(-130, 99, -260));
            Assert.IsNotNull(cell);
            Assert.AreEqual(new GridInt(-3, 2, -5), cell.grid);
            Assert.AreEqual(new PositionInt(-150, 100, -250), cell.position);
            Assert.AreEqual(1, grid.Cells.Count);
        }

        [TestMethod]
        public void AddEntity_Test()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            var entity = GetEntity(new Position(1, 1, 1));
            var cell = grid.AddEntity(entity);
            Assert.IsNotNull(cell);
            Assert.AreEqual(new GridInt(0, 0, 0), cell.grid);
            Assert.AreEqual(new PositionInt(0, 0, 0), cell.position);
            Assert.AreEqual(1, cell.entities.Count);
        }

        [TestMethod]
        public void AddEntity_Test2()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            var entity = GetEntity(new Position(85, 5, 74));
            var cell = grid.AddEntity(entity);
            Assert.IsNotNull(cell);
            Assert.AreEqual(new GridInt(2, 0, 1), cell.grid);
            Assert.AreEqual(new PositionInt(100, 0, 50), cell.position);
            Assert.AreEqual(1, cell.entities.Count);
        }

        [TestMethod]
        public void GetCellsInArea_Test()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, true);
            var interestRange = 100f;
            var cells = grid.GetCellsInArea(new Position(10, 1, 10), interestRange);
            //max edge is  (-90, -99, -90) to (110, 101, 110) which is (-100, -100, -100) to (100, 100, 100);

            int xLower = -50;
            int yLower = -50;
            int zLower = -50;

            int cellCnt = 0;
            for (int cntX = xLower; cntX <= 100; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 100; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 100; cntY += cellSize)
                    {
                        var cellPos = new PositionInt(cntX, cntY, cntZ);
                        Assert.IsTrue(cells.Count > cellCnt, "Cell Count not valid!");
                        Assert.IsTrue(cells.ContainsKey(cellPos), $"Pos {cellPos}");
                        cellCnt++;
                    }
                }
            }
            Assert.AreEqual(cellCnt, cells.Count);
        }

        [TestMethod]
        public void GetCellsInArea_Test2()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            grid.AddEntity(GetEntity(new Position(1, 1, 1)));
            grid.AddEntity(GetEntity(new Position(5, 5, 5)));
            grid.AddEntity(GetEntity(new Position(85, 5, 74)));
            var cells = grid.GetCellsInArea(new Position(10, 1, 10), 100);

            int xLower = -50;
            int yLower = -50;
            int zLower = -50;

            int cellCnt = 0;
            for (int cntX = xLower; cntX <= 100; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 100; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 100; cntY += cellSize)
                    {
                        var cellPos = new PositionInt(cntX, cntY, cntZ);
                        Assert.IsTrue(cells.Count > cellCnt, "Cell Count not valid!");
                        Assert.IsTrue(cells.ContainsKey(cellPos), $"Pos {cellPos}");
                        cellCnt++;
                    }
                }
            }
            Assert.AreEqual(cellCnt, cells.Count);

        }

        [TestMethod]
        public void GetCellsInArea_Test3()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            grid.AddEntity(GetEntity(new Position(1, 1, 1)));
            grid.AddEntity(GetEntity(new Position(5, 5, 5)));
            grid.AddEntity(GetEntity(new Position(126, 5, 100))); //(150, 0, 100)
            var cells = grid.GetCellsInArea(new Position(10, 1, 10), 100);

            int xLower = -50;
            int yLower = -50;
            int zLower = -50;

            int cellCnt = 0;
            for (int cntX = xLower; cntX <= 100; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 100; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 100; cntY += cellSize)
                    {
                        var cellPos = new PositionInt(cntX, cntY, cntZ);
                        Assert.IsTrue(cells.Count > cellCnt, "Cell Count not valid!");
                        Assert.IsTrue(cells.ContainsKey(cellPos), $"Pos {cellPos}");
                        cellCnt++;
                    }
                }
            }
            Assert.AreEqual(cellCnt, cells.Count);
        }

        [TestMethod]
        public void GetCellsInArea_Test4()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            grid.AddEntity(GetEntity(new Position(156, 5, 160))); //(150, 0, 150)
            var cells = grid.GetCellsInArea(new Position(220, 1, 220), 100); //(200, 0, 200)
            //lower range, 220 - 50 = 170. 150?

            int xLower = 150;
            int yLower = -50;
            int zLower = 150;

            int cellCnt = 0;
            int entityCount = 0;
            for (int cntX = xLower; cntX <= 300; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 300; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 100; cntY += cellSize)
                    {
                        var cellPos = new PositionInt(cntX, cntY, cntZ);
                        Assert.IsTrue(cells.Count > cellCnt, "Cell Count not valid!");
                        Assert.IsTrue(cells.ContainsKey(cellPos), $"Pos {cellPos}");
                        entityCount += cells[cellPos].entities.Count;
                        cellCnt++;
                    }
                }
            }

            Assert.AreEqual(cellCnt, cells.Count);
            Assert.AreEqual(entityCount, 1);
        }

        [TestMethod]
        public void GetCellsInArea_Test5()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            grid.AddEntity(GetEntity(new Position(51.53, 1, 72.39003)));
            var cells = grid.GetCellsInArea(new Position(51.33, 1, 72.29003), 50);

            int xLower = 0;
            int yLower = -50;
            int zLower = 0;

            int cellCnt = 0;
            int entityCount = 0;
            for (int cntX = xLower; cntX <= 100; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 100; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 50; cntY += cellSize)
                    {
                        var cellPos = new PositionInt(cntX, cntY, cntZ);
                        Assert.IsTrue(cells.Count > cellCnt, "Cell Count not valid!");
                        Assert.IsTrue(cells.ContainsKey(cellPos), $"Pos {cellPos}");
                        entityCount += cells[cellPos].entities.Count;
                        cellCnt++;
                    }
                }
            }

            Assert.AreEqual(cellCnt, cells.Count);
            Assert.AreEqual(entityCount, 1);
        }

        [TestMethod]
        public void GetCellsInArea_Test6()
        {
            var cellSize = 50;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            grid.AddEntity(GetEntity(new Position(-30, 1, -30)));
            var cells = grid.GetCellsInArea(new Position(-49, 1, -49), 100);

            int xLower = -100;
            int yLower = -50;
            int zLower = -100;

            int cellCnt = 0;
            int entityCount = 0;
            for (int cntX = xLower; cntX <= 50; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 50; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 100; cntY += cellSize)
                    {
                        var cellPos = new PositionInt(cntX, cntY, cntZ);
                        Assert.IsTrue(cells.Count > cellCnt, "Cell Count not valid!");
                        Assert.IsTrue(cells.ContainsKey(cellPos), $"Pos {cellPos}");
                        entityCount += cells[cellPos].entities.Count;
                        cellCnt++;
                    }
                }
            }

            Assert.AreEqual(cellCnt, cells.Count);
            Assert.AreEqual(entityCount, 1);
        }

        [TestMethod]
        public void GetCellsInArea_Test7()
        {
            var cellSize = 100;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            grid.AddEntity(GetEntity(new Position(-30, 1, -30)));
            var cells = grid.GetCellsInArea(new Position(-49, 1, -49), 100);

            int xLower = -100;
            int yLower = -100;
            int zLower = -100;

            int cellCnt = 0;
            int entityCount = 0;
            for (int cntX = xLower; cntX <= 100; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 100; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 100; cntY += cellSize)
                    {
                        var cellPos = new PositionInt(cntX, cntY, cntZ);
                        Assert.IsTrue(cells.Count > cellCnt, "Cell Count not valid!");
                        Assert.IsTrue(cells.ContainsKey(cellPos), $"Pos {cellPos}");
                        entityCount += cells[cellPos].entities.Count;
                        cellCnt++;
                    }
                }
            }

            Assert.AreEqual(cellCnt, cells.Count);
            Assert.AreEqual(entityCount, 1);
        }

        [TestMethod]
        public void GetCellsInArea_Test8()
        {
            var cellSize = 1000000;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            grid.AddEntity(GetEntity(new Position(-30, 1, -30)));
            var cells = grid.GetCellsInArea(new Position(-49, 1, -49), 100);

            int xLower = 0;
            int yLower = 0;
            int zLower = 0;

            int cellCnt = 0;
            int entityCount = 0;
            for (int cntX = xLower; cntX <= 0; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 0; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 0; cntY += cellSize)
                    {
                        var cellPos = new PositionInt(cntX, cntY, cntZ);
                        Assert.IsTrue(cells.Count > cellCnt, "Cell Count not valid!");
                        Assert.IsTrue(cells.ContainsKey(cellPos), $"Pos {cellPos}");
                        entityCount += cells[cellPos].entities.Count;
                        cellCnt++;
                    }
                }
            }

            Assert.AreEqual(cellCnt, cells.Count);
            Assert.AreEqual(entityCount, 1);
        }

        [TestMethod]
        public void GetCellsInArea_Test9()
        {
            var cellSize = 1000000;
            var grid = GetWorldGridWithDefaultLayer(cellSize, false);
            grid.AddEntity(GetEntity(new Position(-30, 1, -30)));
            var cells = grid.GetCellsInArea(new Position(-499999, 1, -50), 100);

            int xLower = -1000000;
            int yLower = 0;
            int zLower = 0;

            int cellCnt = 0;
            int entityCount = 0;
            for (int cntX = xLower; cntX <= 0; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 0; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 0; cntY += cellSize)
                    {
                        var cellPos = new PositionInt(cntX, cntY, cntZ);
                        Assert.IsTrue(cells.Count > cellCnt, "Cell Count not valid!");
                        Assert.IsTrue(cells.ContainsKey(cellPos), $"Pos {cellPos}");
                        entityCount += cells[cellPos].entities.Count;
                        cellCnt++;
                    }
                }
            }

            Assert.AreEqual(cellCnt, cells.Count);
            Assert.AreEqual(entityCount, 1);
        }

    }
}