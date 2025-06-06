﻿using Fullerenes.Server.Factories.Factories;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Objects.Services;
using Fullerenes.Server.Objects.Services.ServicesImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fullerenes.Server.Objects.Services.ServicesImpl.FileService;

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
                0, 0, 0, new([3000], null),
                1, 5, 360, 360, 360,
                3, 2.5f);

            var factory = new SystemOSIFactoryCreator()
                .CreateSystemFactory(request, 1_000);

            var octree = factory.GenerateOctree();

            var area = factory.GenerateLimitedArea(0);

            var path = _adapter.Write([ area ], _fileName, _subFolder);

            Console.WriteLine(path);
        }
    }
}
