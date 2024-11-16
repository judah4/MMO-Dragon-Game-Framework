using Mmogf.Servers.Shared;

namespace Mmogf.Servers.Tests.Worlds
{
    [TestClass]
    public class EntityIdTests
    {

        [TestMethod]
        public void EntityId_Test()
        {
            var entityId = new EntityId(101);
            Assert.AreEqual(101, entityId.Id);
            Assert.IsTrue(entityId.IsValid());
        }

        [TestMethod]
        public void EntityId_Invalid_Test()
        {
            var entityId = new EntityId();
            Assert.AreEqual(0, entityId.Id);
            Assert.IsFalse(entityId.IsValid());
        }

        [TestMethod]
        public void EntityId_Invalid2_Test()
        {
            var entityId = new EntityId(0);
            Assert.AreEqual(0, entityId.Id);
            Assert.IsFalse(entityId.IsValid());
        }

    }
}