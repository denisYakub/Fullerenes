using Fullerenes.Server.Factories.Factories;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Services.IServices;
using Fullerenes.Server.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fullerenes.Server.Services.Services.FileService;

namespace FullerenesServerTests
{
    [TestClass]
    public class FileServiceTest
    {
        private string _fileName = "Series_test";
        private string _folder = Path.Combine(AppContext.BaseDirectory, "Saves");
        private string _subFolder = "Gen_test";

        private IFileService _adapter;

        [TestInitialize]
        public void Init()
        {
            _adapter = new FileService(_folder);
        }

        [TestMethod]
        public void TestWriteMethod()
        {
            CreateFullerenesAndLimitedAreaRequest request = new(
                0, 0, 0, new([100], null),
                1, 5, 360, 360, 360,
                3, 1.5f);

            var factory = new SystemOSIFactoryCreator()
                .CreateSystemFactory(request, 1, 100);

            var octree = factory.GenerateOctree();

            var area = factory.GenerateLimitedArea(0, octree);

            var path = _adapter.Write([ area ], _fileName, _subFolder);

            Console.WriteLine(path);
        }

        [TestMethod]
        public void TestReadMethod()
        {
            AreaMainInfo area = _adapter.ReadMainInfo(_fileName, _subFolder);

            Assert.IsTrue(
                area.Params[0].value == 100 && 
                area.Fullerenes.Count() == 100);
        }
    }
}
