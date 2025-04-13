using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace FullerenesServerTests
{
    [TestClass]
    public class SphereLimitedAreaTest
    {

        static int numberOfSeries = 100;
        static int numberOfFullerenes = 1_000_000;

        static float areaX = 0, areaY = 0, areaZ = 0, areaR = 300;

        private LimitedArea _limitedArea;
        private Octree<Parallelepiped, Fullerene> _octree; 

        [TestMethod]
        public void TestGenerateFullerenesMethod()
        {

            _octree = new Octree<Parallelepiped, Fullerene>(
                new Parallelepiped
                {
                    Center = new(areaX, areaY, areaZ),
                    Height = 2 * areaR,
                    Width = 2 * areaR,
                    Length = 2 * areaR,
                },
                numberOfSeries);

            _octree.GenerateRegions(Parallelepiped.Split8Parts, p => p.Width > 3 * 10);

            static IcosahedronFullerene CreateIcosaherdonFullerene()
            {
                return new IcosahedronFullerene(
                    -areaR, areaR, -areaR, areaR, -areaR, areaR,
                    360, 360, 360,
                    1, 10,
                    3, 1.5f);
            }

            List<Fullerene>[] areas = new List<Fullerene>[numberOfSeries];

            LimitedArea.ClearOctreeCollection = true;

            var timeBefore = DateTime.Now;

            Parallel.For(0, numberOfSeries, i => {

                var limitedArea = new SphereLimitedArea(
                0, 0, 0, 3000,
                _octree, i,
                CreateIcosaherdonFullerene);

                limitedArea.StartGeneration(numberOfFullerenes);

                areas[i] = limitedArea.Fullerenes.ToList();
            });

            var timeAfter = DateTime.Now;

            Assert.IsTrue((timeBefore - timeAfter).Minutes < 5);
        }
    }
}
