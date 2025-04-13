using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;

namespace Fullerenes.Server.DataBase
{
    [Table("sp_data")]
    public class SpData
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column("super_id")]
        public long Id { get; set; }
        [Required]
        [Column("path_to_file")]
        public string FilePath { get; set; }
        [NotMapped]
        public static string FolderNameBin { get; private set; } = "OutPutFilesBin";
        [NotMapped]
        public string FileName {  get; init; }

        public SpData(
            LimitedArea area, IEnumerable<Fullerene> fullerenes, 
            int series, long n)
        {
            ArgumentNullException.ThrowIfNull(fullerenes);

            FileName = "Generation_" + n + '_' + series + ".txt";
            FilePath = GenerateDataFileBin(area, fullerenes, FileName);
        }

        public SpData() : this (null, [], -1, -1) { }

        public static string GenerateDataFileBin(LimitedArea area, IEnumerable<Fullerene> fullerenes, string fileName)
        {
            ArgumentNullException.ThrowIfNull(area);
            ArgumentNullException.ThrowIfNull(fullerenes);

            string fullFolderPath = Path.Combine(
                AppContext.BaseDirectory,
                FolderNameBin);

            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            string fullPath = Path.Combine(fullFolderPath, fileName);

            area.Fullerenes = fullerenes.ToImmutableArray();

            var areaBytes = MessagePackSerializer.Serialize(area);

            File.WriteAllBytes(fullPath, areaBytes);

            return fullPath;
        }

        public static LimitedArea GetDataFormFileBin(string fullFilePath)
        {
            if (!File.Exists(fullFilePath))
                throw new FileNotFoundException();

            var areaBytes = File.ReadAllBytes(fullFilePath);

            LimitedArea area = MessagePackSerializer.Deserialize<LimitedArea>(areaBytes);

            return area;
        }
    }
}
