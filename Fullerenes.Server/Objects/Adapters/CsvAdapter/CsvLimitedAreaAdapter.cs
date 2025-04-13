using CsvHelper;
using CsvHelper.Configuration;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Npgsql.Internal;
using System.Globalization;
using System.Numerics;

namespace Fullerenes.Server.Objects.Adapters.CsvAdapter
{
    public class CsvLimitedAreaAdapter
    {
        private readonly string _filePath;

        public CsvLimitedAreaAdapter(string filePath)
        {
            _filePath = filePath;
        }

        public void Write(IReadOnlyCollection<LimitedArea> areas)
        {
            using var writer = new StreamWriter(_filePath);
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
        }

        public LimitedArea Read(int series)
        {
            using var reader = new StreamReader(_filePath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            csvReader.Read();
            csvReader.ReadHeader();

            while (csvReader.Read())
            {
                var res = csvReader.GetField("CenterF");
            }

            return new SphereLimitedArea();
        }
    }
}
