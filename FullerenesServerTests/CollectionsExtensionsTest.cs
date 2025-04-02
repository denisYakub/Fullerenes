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
                0, 0, 0,
                0, 0, 0,
                5);

            var vertices = fullerne.Vertices.AddMidPoints(fullerne.Faces);

            Assert.AreEqual(vertices.Count, fullerne.Vertices.Count);
        }
    }
}