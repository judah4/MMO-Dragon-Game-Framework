using Mmogf.Core;
using Mmogf.Servers.Worlds;

namespace Mmogf.Servers.Tests.Worlds
{
    [TestClass]
    public class WorldCellTests
    {
        WorldCell GetWorldCell(Position position, int cellSize = 50, int layer = 0)
        {
            return new WorldCell(position, cellSize, layer);
        }

        [TestMethod]
        public void WithinArea_Test()
        {
            var cell = GetWorldCell(Position.Zero, 50);
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
            Assert.IsTrue(cell.WithinArea(new Position(0, 0, 0)));
            Assert.IsTrue(cell.WithinArea(new Position(10, 10, 10)));
            Assert.IsTrue(cell.WithinArea(new Position(24.9999, 24.9999, 24.9999)));
            Assert.IsFalse(cell.WithinArea(new Position(25, 25, 25)), "North and Eastern edges should be in the next cell.");
            Assert.IsTrue(cell.WithinArea(new Position(-25, 0, 0)));
        }

        [TestMethod]
        public void WithinArea_Not_Test()
        {
            var cell = GetWorldCell(Position.Zero, 50);
            Assert.IsFalse(cell.WithinArea(new Position(51, 25, 48)));
        }

        [TestMethod]
        public void WithinArea_Shifted_Test()
        {
            var cellSize = 50;
            //(100, 0, 50), (2,0, 1)
            var cell = GetWorldCell(new Position(2 * cellSize, 0 * cellSize, 1 * cellSize), cellSize);
            Assert.IsTrue(cell.WithinArea(new Position(100, 0, 50)));
            Assert.IsTrue(cell.WithinArea(new Position(101, 24.9999, 55)));
        }

        [TestMethod]
        public void WithinArea_Negative_Test()
        {
            var cellSize = 50;
            //(-50, 0, 0), (-1,0, 0)
            var cell = GetWorldCell(new Position(-1 * cellSize, 0 * cellSize, 0 * cellSize), cellSize);
            Assert.IsTrue(cell.WithinArea(new Position(-50, 1, 5)));
            Assert.IsTrue(cell.WithinArea(new Position(-26, 0, 0)));
        }

        [TestMethod]
        public void WithinArea_Negative_Test2()
        {
            var cellSize = 50;
            //(-50, 0, 0), (-1,0, 0)
            var cell = GetWorldCell(new Position(-2 * cellSize, 0 * cellSize, -2 * cellSize), cellSize);
            Assert.IsTrue(cell.WithinArea(new Position(-125, 1, -125)));
            Assert.IsTrue(cell.WithinArea(new Position(-99, 0, -101)));
        }

        [TestMethod]
        public void WithinArea_CellSize10_Test()
        {
            var cell = GetWorldCell(Position.Zero, 10);
            /*
                                (0,0,5)
                                | 
                                |
                                |
                                |
            (-5,0,0)--------(0,0,0)--------(5,0,0)
                                | 
                                |
                                |
                                |
                                (0,0,-5)
             */
            Assert.IsTrue(cell.WithinArea(new Position(0, 0, 0)));
            Assert.IsTrue(cell.WithinArea(new Position(3, 3, 3)));
            Assert.IsTrue(cell.WithinArea(new Position(4.9999, 4.9999, 4.9999)));
            Assert.IsFalse(cell.WithinArea(new Position(5, 5, 5)), "North and Eastern edges should be in the next cell.");
            Assert.IsTrue(cell.WithinArea(new Position(-5, 0, 0)));
        }


    }
}