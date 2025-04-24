using CsvHelper;
using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Services.IServices;
using System.Globalization;

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

        public LimitedArea Read(string fileName, string? subFolder = null)
        {
            string fullFolderPath;

            if (subFolder is null)
                fullFolderPath = _folderPath;
            else
                fullFolderPath = Path.Combine(_folderPath, subFolder);

            string fullPath = Path.Combine(fullFolderPath, fileName + ".csv");

            using var reader = new StreamReader(fullPath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var flatList = csvReader.GetRecords<FlatArea>();

            /*return new SphereLimitedArea {
                Fullerenes = flatList
                .Select(areaFlat => new IcosahedronFullerene()),
                Octree = null,
                ProduceFullerene = null
            };*/
            throw new NotImplementedException();
        }
    }
}
