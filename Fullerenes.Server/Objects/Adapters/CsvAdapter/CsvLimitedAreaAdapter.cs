using CsvHelper;
using CsvHelper.Configuration;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Npgsql.Internal;
using System.Globalization;
using System.Numerics;

namespace Fullerenes.Server.Objects.Adapters.CsvAdapter
{
    public class CsvLimitedAreaAdapter : ILimitedAreaAdapter
    {
        private readonly string _folderPath;

        public CsvLimitedAreaAdapter(string folderPath)
        {
            _folderPath = folderPath;
        }

        public string Write(IReadOnlyCollection<LimitedArea> areas, string subFolder, string fileName)
        {
            string fullFolderPath = Path.Combine(_folderPath, subFolder);

            string fullPath = Path.Combine(fullFolderPath, fileName + ".csv");

            using var writer = new StreamWriter(fullPath);
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csvWriter.WriteField("SeriesA");
            csvWriter.WriteField("CenterA");
            csvWriter.WriteField("ParamsA");
            csvWriter.WriteField("SizeF");
            csvWriter.WriteField("CenterF");
            csvWriter.WriteField("EulerAnglesF");
            csvWriter.NextRecord();

            foreach (LimitedArea area in areas)
            {
                foreach (Fullerene fullerene in area.Fullerenes)
                {
                    csvWriter.WriteField(area.Series);
                    csvWriter.WriteField(area.Center);
                    csvWriter.WriteField(area.Params);
                    csvWriter.WriteField(fullerene.Size);
                    csvWriter.WriteField(fullerene.Center);
                    csvWriter.WriteField(fullerene.EulerAngles);
                    csvWriter.NextRecord();
                }
            }
            return fullPath;
        }

        public LimitedArea Read(int series, string fileName)
        {
            string fullPath = Path.Combine(_folderPath, fileName);

            using var reader = new StreamReader(fullPath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            csvReader.Read();
            csvReader.ReadHeader();

            while (csvReader.Read())
            {
                var res = csvReader.GetField("CenterF");
            }

            throw new NotImplementedException();
        }
    }
}
