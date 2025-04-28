using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fullerenes.Server.Extensions;
using MathNet.Numerics.Distributions;

namespace FullerenesServerTests
{
    [TestClass]
    public class RandomExtensionsTest
    {
        private readonly static Gamma gamma = new Gamma(3, 2.5);
        private readonly static Random random = new Random();

        private readonly static float min = 1, max = 10;

        [TestMethod]
        public void TestRandomGammaExtension()
        {
            foreach (var randValue in gamma.GetGammaRandoms(min, max).Take(100))
                Assert.IsTrue(randValue <= max && randValue >= min);
        }
    }
}
