using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullerenesServerTests
{
    [TestClass]
    public class SphereLimitedAreaTest
    {
        [TestMethod]
        public void TestGenerateFullerenesMethod()
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

            var limitedArea = new SphereLimitedArea(
                0, 0, 0, 
                3000,
                1_000_000,
                CreateIcosaherdonFullerene);

            var octree = new Octree<Parallelepiped, Fullerene>(
                new Parallelepiped 
                { 
                    Center = limitedArea.Center,
                    Height = 2 * limitedArea.Radius,
                    Width = 2 * limitedArea.Radius,
                    Length = 2 * limitedArea.Radius,
                },
                1);

            octree.GenerateRegions(Parallelepiped.Split8Parts, p => p.Width > 3 * 10);

            var timeBefore = DateTime.Now;

            var fullerenes = limitedArea.GenerateFullerenes(default, octree).Take(1_000_000).ToList();

            var timeAfter = DateTime.Now;

            Assert.IsTrue(fullerenes.Count() == 1_000_000);

            Console.WriteLine((timeAfter - timeBefore).Seconds);
            Assert.IsTrue((timeAfter - timeBefore).Seconds < 30);
        }
    }
}
