using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace FullerenesServerTests
{
    [TestClass]
    public class SphereLimitedAreaTest
    {

        private LimitedArea _limitedArea;
        private Octree<Parallelepiped, Fullerene> _octree; 

        [TestMethod]
        public void TestGenerateFullerenesMethod()
        {
            static IcosahedronFullerene CreateIcosaherdonFullerene(int limitedAreaId = default, int series = default)
            {
                return new IcosahedronFullerene(
                    -3000, 3000, -3000, 3000, -3000, 3000,
                    360, 360, 360,
                    1, 10,
                    3, 1.5f,
                    limitedAreaId, series);
            }

            _limitedArea = new SphereLimitedArea(
                0, 0, 0,
                3000,
                1_000_000,
                CreateIcosaherdonFullerene);

            List<Fullerene>[] fullerenesArr = new List<Fullerene>[100];

            _octree = new Octree<Parallelepiped, Fullerene>(
                new Parallelepiped
                {
                    Center = _limitedArea.Center,
                    Height = 2 * 3000,
                    Width = 2 * 3000,
                    Length = 2 * 3000,
                },
                fullerenesArr.Length);

            _octree.GenerateRegions(Parallelepiped.Split8Parts, p => p.Width > 3 * 10);

            var timeBefore = DateTime.Now;

            Parallel.For(0, fullerenesArr.Length, i => {
                fullerenesArr[i] = _limitedArea.GenerateFullerenes(i, _octree).Take(1_000_000).ToList();
            });
            
            var timeAfter = DateTime.Now;

            Assert.IsTrue(_limitedArea.RealNumberOfFullerenes == 0);

            Console.WriteLine($"Started at {timeBefore}" + '\n' + $"Ended at {timeAfter}");
        }
    }
}
