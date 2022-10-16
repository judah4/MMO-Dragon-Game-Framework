using Grpc.Core;
using MmoGameFramework;
using Mmogf.Core;
using Mmogf.Servers.Worlds;
using static Grpc.Core.Metadata;

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
            var cells = grid.GetCellsInArea(new Position(10, 1, 10), 100);
            Assert.AreEqual(1, cells.Count);
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
            Assert.AreEqual(2, cells.Count);
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
            Assert.AreEqual(1, cells.Count);
        }


    }
}