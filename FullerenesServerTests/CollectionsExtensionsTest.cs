using Fullerenes.Server.Extensions;
using Fullerenes.Server.Objects.Fullerenes;

namespace FullerenesServerTests
{
    [TestClass]
    public class CollectionsExtensionsTest
    {
        [TestMethod]
        public void TestAddMidPointsMethod()
        {
            var fullerne = new IcosahedronFullerene(
                5, 5, 5,
                180, 180, 180,
                5);

            var vertices = fullerne.Vertices;

            //Assert.AreEqual(vertices.Count, fullerne.Vertices.Count);
        }
    }
}