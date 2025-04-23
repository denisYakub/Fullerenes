using System.ComponentModel.DataAnnotations.Schema;

namespace Fullerenes.Server.DataBase
{
    [Table("sp_veiw_group")]
    public class SpGenGroup
    {
        [Column("sp_gen_id")]
        public long SpGenId { get; set; }

        [Column("count_of_generation")]
        public long Series { get; set; }
        [Column("avg_phi")]
        public float? AvgPhi {  get; set; }
    }
}
