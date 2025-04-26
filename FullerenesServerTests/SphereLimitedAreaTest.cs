using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Services.IServices;
using Fullerenes.Server.Services.Services;
using MathNet.Numerics.Distributions;

namespace FullerenesServerTests
{
    [TestClass]
    public class SphereLimitedAreaTest
    {

        static int numberOfSeries = 1;
        static int numberOfFullerenes = 1_000_000;

        static float areaX = 0, areaY = 0, areaZ = 0, areaR = 3000;

        private LimitedArea _limitedArea;
        private IOctree _octree; 

        [TestMethod]
        public void TestGenerateFullerenesMethod()
        {
            var startRegion = new Cube
            {
                Center = new(0, 0, 0),
                Width = areaR * 2,
                Height = areaR * 2,
                Length = areaR * 2,
            };

            _octree = new Octree(startRegion.MaxDepth(3 * 10), numberOfSeries, startRegion);

            var random = new Random();
            var gamma = new Gamma(3, 1.5);

            static IcosahedronFullerene CreateIcosaherdonFullerene(float x, float y, float z, float a, float b, float g, float size)
            {
                return new IcosahedronFullerene(x, y, z, a, b, g, size);
            }

            List<Fullerene>[] areas = new List<Fullerene>[numberOfSeries];

            var timeBefore = DateTime.Now;

            Parallel.For(0, numberOfSeries, i => 
            {
                var limitedArea = new SphereLimitedArea(0, 0, 0, areaR, i, random, gamma)
                { Octree = _octree, ProduceFullerene = CreateIcosaherdonFullerene };

                limitedArea.StartGeneration(numberOfFullerenes, new(360, 360, 360), (1, 10));

                areas[i] = limitedArea.Fullerenes.ToList();
            });

            var timeAfter = DateTime.Now;

            Console.WriteLine(timeBefore + " " + timeAfter);

            Assert.IsTrue((timeBefore - timeAfter).Minutes < 5);
        }
    }
}
