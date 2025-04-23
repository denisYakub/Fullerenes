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

        static int numberOfSeries = 100;
        static int numberOfFullerenes = 1_000_000;

        static float areaX = 0, areaY = 0, areaZ = 0, areaR = 300;

        private LimitedArea _limitedArea;
        private IOctree<Fullerene> _octree; 

        [TestMethod]
        public void TestGenerateFullerenesMethod()
        {

            _octree = new Octree<Fullerene>(
                new Parallelepiped
                {
                    Center = new(areaX, areaY, areaZ),
                    Height = 2 * areaR,
                    Width = 2 * areaR,
                    Length = 2 * areaR,
                },
                numberOfSeries);

            _octree.StartRegionGeneration(10);

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
                var limitedArea = new SphereLimitedArea(0, 0, 0, 3000, i, random, gamma)
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
