using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Factories.Factories;
using Fullerenes.Server.Objects.Adapters;
using Fullerenes.Server.Objects.Adapters.CsvAdapter;
using Fullerenes.Server.Objects.Dtos;

namespace FullerenesServerTests
{
    [TestClass]
    public class SystemAbstractFactoryCreatorTest
    {
        private SystemAbstractFactoryCreator _creator;
        private string _folderPath = Path.Combine(AppContext.BaseDirectory, "testFactoryResults");

        [TestInitialize]
        public void Init()
        {
            ILimitedAreaAdapter adapter = new CsvLimitedAreaAdapter(_folderPath);
            _creator = new SystemOSIFactoryCreator(adapter);
        }

        [TestMethod]
        public void TestCreateMethod()
        {
            CreateFullerenesAndLimitedAreaRequest request = new(
                0, 0, 0,
                new([ 200 ], null),
                1, 5, 
                360, 360, 360,
                3, 1.5f);

            SystemAbstractFactory factory = _creator.CreateSystemFactory(request, 3, 10);

            var octree = factory.GenerateOctree();

            var area = factory.GenerateLimitedArea(0, octree);

            var fullerenes = area.Fullerenes.ToList();

            Assert.IsTrue(area.Fullerenes.Count() == 10);
        }
    }
}
