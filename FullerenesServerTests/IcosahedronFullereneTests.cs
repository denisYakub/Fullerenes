using Fullerenes.Server.Objects.Fullerenes;
using System.Numerics;

namespace FullerenesServerTests
{
    [TestClass]
    public class IcosahedronFullereneTests
    {
        [TestMethod]
        public void TestIntesectMethod()
        {
            var fullerene1 = new IcosahedronFullerene(
               10.5f, -5.3f, 8.2f,
               0, 0, 0,
               90.0f);

            var fullerene2 = new IcosahedronFullerene(
                11.7f, -4.9f, 9.1f,
                0, 0, 0,
                150.0f);

            Assert.IsTrue(fullerene1.Intersect(fullerene2));    
        }
        [TestMethod]
        public void TestContainsMethod()
        {
            var fullerene = new IcosahedronFullerene(
                0, 0, 0,
                0, 0, 0,
                5);

            var f = fullerene.Faces;
            var v = fullerene.Vertices;

            var point = new Vector3(1.82f, -0.39f, -3.64f);

            Assert.IsTrue(fullerene.Contains(point));
        }
        [TestMethod]
        public void TestVolumeMethod()
        {
            var fullerene = new IcosahedronFullerene(
                0, 0, 0,
                0, 0, 0,
                5);

            var edgeLength = fullerene.GetEdgeSize();

            var expectedVolume = (5 * (3 + Math.Sqrt(5)) / 12) * Math.Pow(edgeLength, 3);

            Assert.IsTrue(expectedVolume - fullerene.GenerateVolume() <= 2);
        }
    }
}
