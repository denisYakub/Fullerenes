using Fullerenes.Server.Objects.Adapters.CsvAdapter;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Microsoft.AspNetCore.JsonPatch.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FullerenesServerTests
{
    [TestClass]
    public class CsvAdapterTest
    {
        [TestMethod]
        public void TestWriteMethod()
        {
            string folderPath = Path.Combine(AppContext.BaseDirectory, "TestSaveFile");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, "test-" + DateTime.Now.Minute + ".csv");

            LimitedArea[] areas = new LimitedArea[3];

            var octree = new Octree<Parallelepiped, Fullerene>(
            new Parallelepiped
            {
                    Center = new(0, 0, 0),
                    Height = 2 * 10,
                    Width = 2 * 10,
                    Length = 2 * 10,
                },
                areas.Length);

            octree.GenerateRegions(Parallelepiped.Split8Parts, p => p.Width > 3 * 5);

            static IcosahedronFullerene CreateIcosaherdonFullerene()
            {
                return new IcosahedronFullerene(
                    -10, 10, -10, 10, -10, 10,
                    360, 360, 360,
                    1, 5,
                    3, 1.5f);
            }

            for (int i = 0; i < areas.Length; i++)
            {
                areas[i] = new SphereLimitedArea(
                    0, 0, 0, 10,
                    octree, i,
                    CreateIcosaherdonFullerene);

                areas[i].StartGeneration(5);
            }

            var adapter = new CsvLimitedAreaAdapter(filePath);
            adapter.Write(areas);
        }
        [TestMethod]
        public void TestReadMethod()
        {
            string folderPath = Path.Combine(AppContext.BaseDirectory, "TestSaveFile");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, "test-13.csv");

            var adapter = new CsvLimitedAreaAdapter(filePath);
            var area = adapter.Read(0);
        }
    }
}
