using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fullerenes.Server.DataBase
{
    [Table("sp_gen_id_counter")]
    public class SpGenIdCounter
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("gen_id_current")]
        public long GenIdCurrent { get; set; }
    }
}
