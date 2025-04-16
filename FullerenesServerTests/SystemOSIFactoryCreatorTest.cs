using Fullerenes.Server.Factories.Factories;
using Fullerenes.Server.Objects.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullerenesServerTests
{
    [TestClass]
    public class SystemOSIFactoryCreatorTest
    {
        [TestMethod]
        public void TestGetAreaRadiusMethod()
        {
            AreaAdditionalParamsRequest requestNc = new AreaAdditionalParamsRequest()
            {
                AreaParams = null,
                Nc = 30.5f,
            };

            float r1 = SystemOSIFactoryCreator.GetAreaRadius(requestNc, 1000, 1, 10, 3, 1.5f);

            Assert.IsTrue(r1 == 200);
        }
    }
}
