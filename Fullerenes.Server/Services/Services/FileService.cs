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
        public class FlatArea
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
                    var flatArea = Adapter.AdaptToFlatArea(area);

                    csvWriter.WriteRecords(flatArea);
                }
            }
            return fullPath;
        }

        public AreaMainInfo ReadMainInfo(string fullPath)
        {
            using var reader = new StreamReader(fullPath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var flatList = csvReader.GetRecords<FlatArea>().ToList();

            return Adapter.AdaptToMainInfo(flatList);
        }

        public LimitedArea GetArea(string fullPath)
        {
            using var reader = new StreamReader(fullPath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var flatList = csvReader.GetRecords<FlatArea>().ToList();

            return Adapter.AdaptToLimitedArea(flatList);
        }

        public LimitedArea[] GetAreas(long genId)
        {
            throw new NotImplementedException();
        }
    }
}
