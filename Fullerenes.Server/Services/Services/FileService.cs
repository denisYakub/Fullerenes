using CsvHelper;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Services.IServices;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Numerics;
using static Fullerenes.Server.Services.Services.FileService.AreaMainInfo;

namespace Fullerenes.Server.Services.Services
{
    public class FileService : IFileService
    {
        private class FlatArea
        {
            public required int SeriesA { get; set; }
            public required string CenterA { get; set; }
            public required string ParamsA { get; set; }
            public required float SizeF { get; set; }
            public required string CenterF { get; set; }
            public required string EulerAnglesF { get; set; }
        }

        public class AreaMainInfo
        {
            public class FullereneMainInfo
            {
                public required float Size {  get; set; }
                public required Vector3 Center { get; set; }
                public EulerAngles EulerAngles { get; set; }
            }
            public required AreaTypes AreaType { get; set; }
            public required FullereneTypes FullereneType { get; set; }
            public required int Series { get; set; }
            public required Vector3 Center { get; set; }
            public required (string name, float value)[] Params { get; set; }
            public required IReadOnlyCollection<FullereneMainInfo> Fullerenes { get; set; }
        }

        private readonly string _folderPath;

        public FileService(string folderPath)
        {
            _folderPath = folderPath;
        }

        public string Write(IReadOnlyCollection<LimitedArea> areas, string fileName, string? subFolder = null)
        {
            ArgumentNullException.ThrowIfNull(areas);

            string fullFolderPath;

            if (subFolder is null)
                fullFolderPath = _folderPath;
            else
                fullFolderPath = Path.Combine(_folderPath, subFolder);

            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            string fullPath = Path.Combine(fullFolderPath, fileName + ".csv");

            using var writer = new StreamWriter(fullPath);
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            foreach (LimitedArea area in areas)
            {
                if (area.Fullerenes is not null)
                {
                    var flatArea = area.Fullerenes.Select(fullerene => new FlatArea
                    {
                        SeriesA = area.Series,
                        CenterA = area.Center.ToString(),
                        ParamsA = string.Join(", ", area.Params),
                        SizeF = fullerene.Size,
                        CenterF = fullerene.Center.ToString(),
                        EulerAnglesF = fullerene.EulerAngles.ToString(),
                    });

                    csvWriter.WriteRecords(flatArea);
                }
            }
            return fullPath;
        }

        public AreaMainInfo ReadMainInfo(string fileName, string? subFolder = null)
        {
            string fullFolderPath;

            if (subFolder is null)
                fullFolderPath = _folderPath;
            else
                fullFolderPath = Path.Combine(_folderPath, subFolder);

            string fullPath = Path.Combine(fullFolderPath, fileName + ".csv");

            using var reader = new StreamReader(fullPath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var flatList = csvReader.GetRecords<FlatArea>().ToList();

            int seriesA = flatList.First().SeriesA;

            var flatCenterA = flatList.First().CenterA;

            // Убираем угловые скобки
            flatCenterA = flatCenterA.Trim('<', '>').Replace(',', '.');

            // Разделяем строку на компоненты по пробелу
            string[] components = flatCenterA.Split(' ');

            // Преобразуем компоненты в float и создаем Vector3
            float xA = float.Parse(components[0].Trim(), CultureInfo.InvariantCulture);
            float yA = float.Parse(components[1].Trim(), CultureInfo.InvariantCulture);
            float zA = float.Parse(components[2].Trim(), CultureInfo.InvariantCulture);

            Vector3 centerA = new(xA, yA, zA);

            string flatParams = flatList.First().ParamsA;

            // Разделяем строку по "), (" и убираем скобки
            string[] pairs = flatParams.Trim('(', ')').Split("), (", StringSplitOptions.None);

            // Создаем массив кортежей (string, float)
            var paramsA = pairs.Select(pair =>
            {
                // Разделяем пару на имя и значение
                var parts = pair.Split(", ");
                string name = parts[0];
                float value = float.Parse(parts[1]);

                return (name, value);
            }).ToArray();

            List<FullereneMainInfo> fullerenesA = new List<FullereneMainInfo>(flatList.Count());

            foreach ( FlatArea flatArea in flatList)
            {
                float sizeF = flatArea.SizeF;

                var flatCenterF = flatArea.CenterF;

                // Убираем угловые скобки
                flatCenterF = flatCenterF.Trim('<', '>').Replace(',', '.');

                // Разделяем строку на компоненты по пробелу
                string[] componentsF = flatCenterF.Split(' ');

                // Преобразуем компоненты в float и создаем Vector3
                float xF = float.Parse(componentsF[0].Trim(), CultureInfo.InvariantCulture);
                float yF = float.Parse(componentsF[1].Trim(), CultureInfo.InvariantCulture);
                float zF = float.Parse(componentsF[2].Trim(), CultureInfo.InvariantCulture);

                Vector3 centerF = new(xF, yF, zF);

                var flatEulerAnglesF = flatArea.EulerAnglesF;

                // Убираем угловые скобки
                flatEulerAnglesF = flatEulerAnglesF.Trim('<', '>').Replace(',', '.');

                // Разделяем строку на компоненты по пробелу
                componentsF = flatEulerAnglesF.Split(' ');

                // Преобразуем компоненты в float и создаем Vector3
                float aF = float.Parse(componentsF[0].Trim(), CultureInfo.InvariantCulture);
                float bF = float.Parse(componentsF[1].Trim(), CultureInfo.InvariantCulture);
                float gF = float.Parse(componentsF[2].Trim(), CultureInfo.InvariantCulture);

                EulerAngles eulerAnglesF = new(aF, bF, gF);

                fullerenesA.Add(new FullereneMainInfo
                {
                    Size = sizeF,
                    Center = centerF,
                    EulerAngles = eulerAnglesF,
                });
            }

            return new AreaMainInfo
            {
                AreaType = AreaTypes.Sphere,
                Series = seriesA,
                Center = centerA,
                Params = paramsA,
                Fullerenes = fullerenesA,
                FullereneType = FullereneTypes.Icosahedron,
            };
        }

        public LimitedArea GetArea(string fileName, string? subFolder = null)
        {
            throw new NotImplementedException();
        }
    }
}
