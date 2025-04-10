using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Fullerenes.Server.DataBase
{
    [Table("sp_data")]
    public class SpData
    {
        [Key]
        [Column("super_id")]
        public long Id { get; set; }
        [Required]
        [Column("path_to_file")]
        public string FilePath { get; set; }
        [NotMapped]
        public static string FolderName { get; private set; } = "OutPutFiles";
        [NotMapped]
        public string FileName {  get; init; }

        public SpData(
            LimitedArea area, IEnumerable<Fullerene> fullerenes, 
            int series, long n)
        {
            ArgumentNullException.ThrowIfNull(fullerenes);

            FileName = "Generation_" + n + '_' + series + ".txt";
            FilePath = GenerateDataFile(area, fullerenes);
        }

        public SpData() : this (null, [], -1, -1) { }

        private string GenerateDataFile(LimitedArea area, IEnumerable<Fullerene> fullerenes)
        {
            string fullFolderPath = Path.Combine(
                AppContext.BaseDirectory, 
                FolderName);

            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            string fullPath = Path.Combine(fullFolderPath, FileName);

            using (var stream = File.CreateText(fullPath))
            {
                stream.WriteLine("Limited Area:");
                stream.WriteLine(area.ToString());
            }

            using (var stream = File.AppendText(fullPath))
            {
                stream.WriteLine("Fullerenes: [");

                foreach (var fullerene in fullerenes)
                {
                    stream.WriteLine(fullerene.ToString());
                }

                stream.WriteLine(']');
            }

            return fullPath;
        }
    }
}
