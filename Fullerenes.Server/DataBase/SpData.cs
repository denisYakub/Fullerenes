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
    public class SpData(string filePath)
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column("super_id")]
        public long Id { get; set; }
        [Required]
        [Column("path_to_file")]
        public string FilePath { get; set; } = filePath;
        [NotMapped]
        public static string FolderNameBin { get; private set; } = "OutPutFilesBin";

        public SpData() : this ("") { }

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
