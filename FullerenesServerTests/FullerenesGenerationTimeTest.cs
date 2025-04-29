using Fullerenes.Server.Extensions;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullerenesServerTests
{
    [TestClass]
    public class FullerenesGenerationTimeTest
    {
        private readonly static int numberOfFullerenes = 1_000_000;

        private readonly static Fullerene[] randFullerenes = new Fullerene[numberOfFullerenes];

        private static LimitedArea area = null;

        [TestInitialize]
        public void Init()
        {
            var rand = new Random().GetEvenlyRandoms(-500, 500).GetEnumerator();
            var gamm = new Gamma(3, 2.5).GetGammaRandoms(1, 5).GetEnumerator();

            for (int i = 0; i < numberOfFullerenes; i++)
            {
                if (rand.MoveNext() && gamm.MoveNext())
                    randFullerenes[i] = new IcosahedronFullerene(
                        rand.Current, rand.Current, rand.Current,
                        360, 360, 360,
                        gamm.Current);
            }

            area = new SphereLimitedArea(0, 0, 0, 500, 0, null, null) { Octree = null, ProduceFullerene = null };

            Console.WriteLine($"Number of fullerenes: {numberOfFullerenes}");
        }

        [TestMethod]
        public void TestLimitedAreaGetFieldsTime()
        {
            var timeBefore = DateTime.Now;

            for (int i = 0; i < numberOfFullerenes; i++)
            {
                var center = area.Center;
                var param = area.Params;
                var reTry = LimitedArea.RetryCountMax;
                var rand = area.Random;
                var gamm = area.Gamma;
                var outerR = area.OuterRadius;
            }

            var timeAfter = DateTime.Now;

            Console.WriteLine($"Area get fields: {timeAfter - timeBefore}");
        }

        [TestMethod]
        public void TestLimitedAreaContainMethodTime()
        {
            var timeBefore = DateTime.Now;

            for (int i = 0; i < numberOfFullerenes; i++)
                area.Contains(randFullerenes[i]);

            var timeAfter = DateTime.Now;

            Console.WriteLine($"Area contain method: {timeAfter - timeBefore}");
        }
    }
}
