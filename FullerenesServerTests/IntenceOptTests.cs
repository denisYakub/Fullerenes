using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Objects.Services;
using Fullerenes.Server.Objects.Services.ServicesImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullerenesServerTests
{
    [TestClass]
    public class IntenceOptTests
    {
        private string _fileName = "Series_test.csv";
        private string _folder = Path.Combine(AppContext.BaseDirectory, "Saves");
        private string _subFolder = "Gen_test";

        private IFileService _adapter;

        [TestInitialize]
        public void Init()
        {
            _adapter = new FileService(_folder);
        }

        [TestMethod]
        public void Test_GenerateIntensOpt_Method()
        {
            var folder = Path.Combine(_folder, _subFolder);
            var file = Path.Combine(folder, _fileName);

            var service = new CreateService(null, _adapter);

            var (q, I) = service.GenerateIntensOpt(file, 0.02f, 5, 150);

            Assert.IsTrue(q.Any());
            Assert.IsTrue(I.Any());
        }
    }
}
