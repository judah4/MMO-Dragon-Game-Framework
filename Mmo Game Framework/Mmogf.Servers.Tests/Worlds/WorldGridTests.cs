using Grpc.Core;
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
        public void GetCellsInArea_Test()
        {
            var cellSize = 50;
            var grid = GetWorldGrid(cellSize, true);
            var cells = grid.GetCellsInArea(new Position(10, 1, 10), 100);
            Assert.AreEqual(1, cells.Count);
        }


    }
}