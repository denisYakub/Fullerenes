using Fullerenes.Server.DataBase;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullerenesServerTests
{
    [TestClass]
    public class SpDataTest
    {
        private SpData _spData;
        
        [TestMethod]
        public void TestGenerateDataFileMethod()
        {
            LimitedArea area = new SphereLimitedArea(
                0, 0, 0, 10,
                10, null);

            Fullerene[] fullerenes = [
                new IcosahedronFullerene(1, 1, 1, 0, 0, 0, 5), 
                new IcosahedronFullerene(2, 2, 2, 0, 0, 0, 10),
                new IcosahedronFullerene(3, 3, 3, 0, 0, 0, 15),
            ];

            _spData = new(area, fullerenes, 0, 0);
        }
    }
}
