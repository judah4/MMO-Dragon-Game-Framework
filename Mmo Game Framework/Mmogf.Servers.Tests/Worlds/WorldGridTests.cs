using MmoGameFramework;
using Mmogf.Core;
using Mmogf.Servers.Worlds;

namespace Mmogf.Servers.Tests.Worlds
{
    [TestClass]
    public class WorldGridTests
    {
        WorldGrid GetWorldGrid(int cellSize, bool addEntity)
        {
            var grid =  new WorldGrid(cellSize);

            if(addEntity)
            {
                grid.AddEntity(GetEntity(new Position(1, 1, 1)));
            }

            return grid;
        }

        Entity GetEntity(Position postion)
        {
            var entity = new Entity(1, new Dictionary<short, byte[]>()
            {
                { EntityType.ComponentId, MessagePack.MessagePackSerializer.Serialize(new EntityType() { Name ="Npc" }) },
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
            var grid = GetWorldGrid(cellSize, false);
            var cell = grid.GetCell(new Position(1, 1, 1));
            Assert.IsNotNull(cell);
            Assert.AreEqual(new Position(0, 0, 0), cell.Position);
            Assert.AreEqual(1, grid.Cells.Count);
            Assert.IsTrue(grid.Cells.TryGetValue((0,0,0), out WorldCell? c));
        }

        [TestMethod]
        public void GetCell_Test2()
        {
            var cellSize = 50;
            var grid = GetWorldGrid(cellSize, false);
            var cell = grid.GetCell(new Position(25, 1, 25));
            Assert.IsNotNull(cell);
            Assert.AreEqual(new Position(50, 0, 50), cell.Position);
            Assert.AreEqual(1, grid.Cells.Count);
            Assert.IsTrue(grid.Cells.TryGetValue((1, 0, 1), out WorldCell? c));
        }

        [TestMethod]
        public void GetCell_Test3()
        {
            var cellSize = 25;
            var grid = GetWorldGrid(cellSize, false);
            var cell = grid.GetCell(new Position(15, 1, 15));
            Assert.IsNotNull(cell);
            Assert.AreEqual(new Position(25, 0, 25), cell.Position);
            Assert.AreEqual(1, grid.Cells.Count);
            Assert.IsTrue(grid.Cells.TryGetValue((1, 0, 1), out WorldCell? c));
        }


        [TestMethod]
        public void AddEntity_Test()
        {
            var cellSize = 50;
            var grid = GetWorldGrid(cellSize, false);
            var entity = GetEntity(new Position(1, 1, 1));
            var cell = grid.AddEntity(entity);
            Assert.IsNotNull(cell);
            Assert.AreEqual(new Position(0,0,0), cell.Position);
            Assert.AreEqual(1, cell.EntityCount);
        }

        [TestMethod]
        public void AddEntity_Test2()
        {
            var cellSize = 50;
            var grid = GetWorldGrid(cellSize, false);
            var entity = GetEntity(new Position(85, 5, 74));
            var cell = grid.AddEntity(entity);
            Assert.IsNotNull(cell);
            Assert.AreEqual(new Position(100, 0, 50), cell.Position);
            Assert.AreEqual(1, cell.EntityCount);
        }

        [TestMethod]
        public void GetCellsInArea_Test()
        {
            var cellSize = 50;
            var grid = GetWorldGrid(cellSize, true);
            var interestRange = 100f;
            var cells = grid.GetCellsInArea(new Position(10, 1, 10), interestRange);
            //max edge is  (-90, -99, -90) to (110, 101, 110) which is (-100, -100, -100) to (100, 100, 100);

            int xLower = -50;
            int yLower = -50;
            int zLower = -50;

            int cellCnt = 0;
            for (int cntX = xLower; cntX <= 50; cntX+=cellSize)
            {
                for (int cntZ = zLower; cntZ <= 50; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 50; cntY += cellSize)
                    {
                        Assert.IsTrue(cells.Count > cellCnt);
                        Assert.AreEqual(new Position(cntX, cntY, cntZ), cells[cellCnt].Position);
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
            var grid = GetWorldGrid(cellSize, false);
            grid.AddEntity(GetEntity(new Position(1, 1, 1)));
            grid.AddEntity(GetEntity(new Position(5, 5, 5)));
            grid.AddEntity(GetEntity(new Position(85, 5, 74)));
            var cells = grid.GetCellsInArea(new Position(10, 1, 10), 100);

            int xLower = -50;
            int yLower = -50;
            int zLower = -50;

            int cellCnt = 0;
            for (int cntX = xLower; cntX <= 50; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 50; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 50; cntY += cellSize)
                    {
                        Assert.IsTrue(cells.Count > cellCnt);
                        Assert.AreEqual(new Position(cntX, cntY, cntZ), cells[cellCnt].Position);
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
            var grid = GetWorldGrid(cellSize, false);
            grid.AddEntity(GetEntity(new Position(1, 1, 1)));
            grid.AddEntity(GetEntity(new Position(5, 5, 5)));
            grid.AddEntity(GetEntity(new Position(126, 5, 100))); //(150, 0, 100)
            var cells = grid.GetCellsInArea(new Position(10, 1, 10), 100);

            int xLower = -50;
            int yLower = -50;
            int zLower = -50;

            int cellCnt = 0;
            for (int cntX = xLower; cntX <= 50; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 50; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 50; cntY += cellSize)
                    {
                        Assert.IsTrue(cells.Count > cellCnt);
                        Assert.AreEqual(new Position(cntX, cntY, cntZ), cells[cellCnt].Position);
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
            var grid = GetWorldGrid(cellSize, false);
            grid.AddEntity(GetEntity(new Position(156, 5, 160))); //(150, 0, 150)
            var cells = grid.GetCellsInArea(new Position(220, 1, 220), 100); //(200, 0, 200)
            //lower range, 220 - 50 = 170. 150?

            int xLower = 150;
            int yLower = -50;
            int zLower = 150;

            int cellCnt = 0;
            for (int cntX = xLower; cntX <= 250; cntX += cellSize)
            {
                for (int cntZ = zLower; cntZ <= 250; cntZ += cellSize)
                {
                    for (int cntY = yLower; cntY <= 50; cntY += cellSize)
                    {
                        Assert.IsTrue(cells.Count > cellCnt);
                        Assert.AreEqual(new Position(cntX, cntY, cntZ), cells[cellCnt].Position);
                        cellCnt++;
                    }
                }
            }

            Assert.AreEqual(cellCnt, cells.Count);
        }


    }
}