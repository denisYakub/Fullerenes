using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using MathNet.Numerics.Distributions;

namespace FullerenesServerTests
{
    [TestClass]
    public class SphereLimitedAreaTest
    {
        private readonly static int numberOfFullerenes = 500_000;

        private readonly static int numberOfSeries = 1;

        private readonly static float areaX = 0, areaY = 0, areaZ = 0, areaR = 3000;

        private readonly static float shape = 3, scale = 2.5f;

        private readonly static float minF = 1, maxF = 5;

        [TestMethod]
        public void TestGenerateFullerenesMethod()
        {
            var startRegion = new CubeRegion
            {
                Center = new(areaX, areaY, areaZ),
                Edge = areaR * 2,
            };

            var random = new Random();
            var gamma = new Gamma(shape, scale);

            static IcosahedronFullerene CreateIcosaherdonFullerene(float x, float y, float z, float a, float b, float g, float size)
            {
                return new IcosahedronFullerene(x, y, z, a, b, g, size);
            }

            IOctree CreateOctree()
            {
                return new Octree(startRegion.MaxDepth(3 * maxF), startRegion);
            }

            var timeBefore = DateTime.Now;

            Parallel.For(0, numberOfSeries, (i) =>
            {
                IOctree octree = new Octree(startRegion.MaxDepth(3 * maxF), startRegion);

                var limitedArea = new SphereLimitedArea(areaX, areaY, areaZ, areaR, i)
                {
                    ProduceOctree = CreateOctree, ProduceFullerene = CreateIcosaherdonFullerene,
                    Random = random, Gamma = gamma
                };

                limitedArea.StartGeneration(
                    numberOfFullerenes, 
                    new() { ProperRotationAngle = 360, NutatioAngle = 360, PraecessioAngle = 360}, 
                    (minF, maxF));
            });

            var timeAfter = DateTime.Now;

            Console.WriteLine($"{timeAfter - timeBefore}");

            Assert.IsTrue((timeBefore - timeAfter).Minutes < 5);
        }
    }
}
