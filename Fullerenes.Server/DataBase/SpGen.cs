using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Fullerenes.Server.DataBase
{
    [Table("sp_gen")]
    public class SpGen(AreaTypes area, FullereneTypes fullerene, int series, long n, long spDataId)
    {
        [Key]
        [Column("id")]
        public long Id {  get; set; }
        [Column("series")]
        public int Series { get; set; } = series;
        [AllowNull]
        [Column("phi")]
        public required float? Phi {  get; set; }
        [Column("number_of_generation")]
        public long N { get; set; } = n;
        [Column("area_type")]
        public AreaTypes Area { get; set; } = area;
        [Column("fullerene_type")]
        public FullereneTypes Fullerene { get; set; } = fullerene;
        [ForeignKey("super_id")]
        [Column("super_id")]
        public long SpDataId { get; set; } = spDataId;

        public SpGen() : this(0, 0, -1, -1, -1) { }
    }
}
