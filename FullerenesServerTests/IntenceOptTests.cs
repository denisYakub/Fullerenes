using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Services.IServices;
using Fullerenes.Server.Services.Services;
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

            LimitedArea areas = _adapter.GetArea(file);
        }
    }
}
