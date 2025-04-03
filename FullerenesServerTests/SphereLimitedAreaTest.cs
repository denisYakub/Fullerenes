using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullerenesServerTests
{
    [TestClass]
    public class SphereLimitedAreaTest
    {

        private LimitedArea _limitedArea;
        private Octree<Parallelepiped, Fullerene> _octree;

        /*[TestInitialize]
        public void InitSphereLimitedArea()
        {

            IcosahedronFullerene CreateIcosaherdonFullerene(int limitedAreaId = default, int series = default)
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
        }

        [TestInitialize]
        public void InitOctree()
        {
            _octree = new Octree<Parallelepiped, Fullerene>(
                new Parallelepiped
                {
                    Center = _limitedArea.Center,
                    Height = 2 * 3000,
                    Width = 2 * 3000,
                    Length = 2 * 3000,
                },
                1);

            _octree.GenerateRegions(Parallelepiped.Split8Parts, p => p.Width > 3 * 10);
        }

        [TestCleanup]
        public void ClearOctree()
        {
            _octree.ClearSpecificThread(1);
        }*/

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

            List<Fullerene>[] fullerenesArr = new List<Fullerene>[5];

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

                _octree.ClearCurrentThreadCollection(i);
            });
            
            var timeAfter = DateTime.Now;

            foreach (var fullerenes  in fullerenesArr)
                Assert.IsTrue(fullerenes.Count() == 1_000_000);

            Console.WriteLine($"Started at {timeBefore}" + '\n' + "Ended at {timeAfter}");

            _octree.ClearAllThread();
        }

        /*[TestMethod]
        public void TestAvgFullereneGenerationTime()
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

            _octree = new Octree<Parallelepiped, Fullerene>(
                new Parallelepiped
                {
                    Center = _limitedArea.Center,
                    Height = 2 * 3000,
                    Width = 2 * 3000,
                    Length = 2 * 3000,
                },
                1);

            _octree.GenerateRegions(Parallelepiped.Split8Parts, p => p.Width > 3 * 10);
            var stopWatch = new Stopwatch();
            List<double> times = new List<double>();

            var fullerenes = _limitedArea.GenerateFullerenes(default, _octree);

            foreach (var fullerene in fullerenes.Take(1_000_000))
            {
                stopWatch.Restart();
                stopWatch.Stop();
                times.Add(stopWatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000);
            }

            Console.WriteLine("Avg time is: " + times.Average());
        }*/
    }
}
