using Fullerenes.Server.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FullerenesServerTests
{
    [TestClass]
    public class FigureCollisionTest
    {
        private (Vector3 center, float r) _shpere1;
        private (Vector3 center, float r) _shpere2;

        [TestInitialize]
        public void InitSpheres()
        {
            _shpere1 = new(new(0, 0, 0), 5);
            _shpere2 = new(new(5, 0, 5), 3);
        }

        [TestMethod]
        public void TestSpheresInsideMethod()
        {
            Assert.IsFalse(
                FiguresCollision.SpheresInside(
                    _shpere1.center, _shpere1.r,
                    _shpere2.center, _shpere2.r)
                );
        }
        [TestMethod]
        public void TestSpheresIntersectMethod()
        {
            Assert.IsTrue(
               FiguresCollision.SpheresIntersect(
                   _shpere1.center, _shpere1.r,
                   _shpere2.center, _shpere2.r)
               );
        }
    }
}
