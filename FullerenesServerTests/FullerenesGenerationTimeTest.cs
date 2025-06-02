using Fullerenes.Server.Extensions;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
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
            using var rand = new Random().GetEvenlyRandoms(-500, 500).GetEnumerator();
            using var gamm = new Gamma(3, 2.5).GetGammaRandoms(1, 5).GetEnumerator();

            for (int i = 0; i < numberOfFullerenes; i++)
            {
                if (rand.MoveNext() && gamm.MoveNext())
                    randFullerenes[i] = new IcosahedronFullerene(
                        rand.Current, rand.Current, rand.Current,
                        360, 360, 360,
                        gamm.Current);
            }

            area = new SphereLimitedArea(0, 0, 0, 500, 0) 
            {
                ProduceOctree = null, ProduceFullerene = null,
                Random = null, Gamma = null
            };

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

        [TestMethod]
        public void TestFullereneIntersectMethodTime()
        {
            TimeSpan? biggestRunTime = null;
            int iB = 0;
            int jB = 0;

            for (int i = 1; i < numberOfFullerenes; i++)
                for (int j = numberOfFullerenes - 1; j > -1; j--)
                {
                    if (i == j)
                        break;

                    DateTime timeBefore = DateTime.Now;

                    randFullerenes[i].Intersect(randFullerenes[j]);

                    DateTime timeAfter = DateTime.Now;

                    var time = timeAfter - timeBefore;

                    if (biggestRunTime is null)
                    {
                        biggestRunTime = time;
                    }
                    else if (biggestRunTime < time)
                    {
                        biggestRunTime = time;

                        iB = i;
                        jB = j;
                    }
                }

            Console.WriteLine($"Longest Intersect: {biggestRunTime}");
            Console.WriteLine($"f1: size{randFullerenes[iB].Size}, angles: {randFullerenes[iB].EulerAngles}, center:{randFullerenes[iB]}");
            Console.WriteLine($"f2: size{randFullerenes[jB].Size}, angles: {randFullerenes[jB].EulerAngles}, center:{randFullerenes[jB]}");
        }

        [TestMethod]
        public void TestGetRandValueMethodTime()
        {
            float min = 1, max = 5;
            using var valueR = new Random().GetEvenlyRandoms(min, max).GetEnumerator();
            using var valueG = new Gamma(3, 2.5).GetGammaRandoms(min, max).GetEnumerator();

            for (int i = 0; i < numberOfFullerenes; i++)
            {
                if(valueR.MoveNext() && valueG.MoveNext())
                {
                    Assert.IsTrue(valueR.Current >= min && valueR.Current <= max);
                    Assert.IsTrue(valueG.Current >= min && valueG.Current <= max);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void TestOctreeAddMethodTime()
        {
            CubeRegion region = new CubeRegion() { Center = new(0, 0, 0), Edge = 1000 };

            IOctree octree = new Octree(region.MaxDepth(3 * 5), region);

            int wasAdded = 0, wasNotAdded = 0;

            var timeBefore = DateTime.Now;

            for (int i = 0; i < numberOfFullerenes; i++)
            {
                if (octree.Add(randFullerenes[i]))
                    wasAdded++;
                else
                    wasNotAdded++;
            }

            var timeAfter = DateTime.Now;

            Console.WriteLine($"Addig to octree method: {timeAfter - timeBefore}");
            Console.WriteLine($"Added: {wasAdded}, Wasnt added: {wasNotAdded}");
        }
    }
}
