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
        private readonly static int numberOfFullerenes = 300_000;

        private readonly static int numberOfSeries = 1;

        private readonly static float areaX = 0, areaY = 0, areaZ = 0, areaR = 3000;

        private readonly static float shape = 3, scale = 2.5f;

        private readonly static float minF = 1, maxF = 5;

        [TestMethod]
        public void TestGenerateFullerenesMethod()
        {
            var startRegion = new Cube
            {
                Center = new(areaX, areaY, areaZ),
                Width = areaR * 2,
                Height = areaR * 2,
                Length = areaR * 2,
            };

            IOctree octree = new Octree(startRegion.MaxDepth(3 * maxF), numberOfSeries, startRegion);

            var random = new Random();
            var gamma = new Gamma(shape, scale);

            static IcosahedronFullerene CreateIcosaherdonFullerene(float x, float y, float z, float a, float b, float g, float size)
            {
                return new IcosahedronFullerene(x, y, z, a, b, g, size);
            }

            Fullerene[][] areas = new Fullerene[numberOfSeries][];

            for (int i = 0; i < numberOfSeries; i++)
                areas[i] = new Fullerene[numberOfFullerenes];

            var timeBefore = DateTime.Now;

            Parallel.For(0, numberOfSeries, (i) =>
            {
                var limitedArea = new SphereLimitedArea(areaX, areaY, areaZ, areaR, i, random, gamma)
                { Octree = octree, ProduceFullerene = CreateIcosaherdonFullerene };

                limitedArea.StartGeneration(numberOfFullerenes, new(360, 360, 360), (minF, maxF));

                areas[i] = limitedArea.Fullerenes.ToArray();
            });

            var timeAfter = DateTime.Now;

            Console.WriteLine($"{timeAfter - timeBefore}");

            Assert.IsTrue((timeBefore - timeAfter).Minutes < 5);
        }
    }
}
