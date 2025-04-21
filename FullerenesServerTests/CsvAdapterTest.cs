using Fullerenes.Server.Objects.Adapters.CsvAdapter;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
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
        private string _folderPath = Path.Combine(AppContext.BaseDirectory, "AdapterFiles");
        string _subFolder = DateTime.Now.Day + "_" + DateTime.Now.Month;
        string _fileName = "save_" + DateTime.Now.Minute;

        [TestMethod]
        public void TestWriteMethod()
        {
            /*LimitedArea[] areas = new LimitedArea[3];

            var octree = new Octree<Fullerene>(
                new Parallelepiped
                {
                    Center = new(0, 0, 0),
                    Height = 2 * 10,
                    Width = 2 * 10,
                    Length = 2 * 10,
                },
                areas.Length);

            octree.StartRegionGeneration(5);

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
                areas[i] = new SphereLimitedArea(0, 0, 0, 10, i)
                { Octree = octree, ProduceFullerene = CreateIcosaherdonFullerene };

                areas[i].StartGeneration(5);
            }

            var adapter = new CsvLimitedAreaAdapter(_folderPath);
            adapter.Write(areas, _fileName, _subFolder);*/
        }
        [TestMethod]
        public void TestReadMethod()
        {
            var adapter = new CsvLimitedAreaAdapter(_folderPath);
            LimitedArea area = adapter.Read("save_10", _subFolder);
        }
    }
}
