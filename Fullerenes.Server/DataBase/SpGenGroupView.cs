using System.ComponentModel.DataAnnotations.Schema;

namespace Fullerenes.Server.DataBase
{
    public class SpGenGroupView
    {
        [Column("sp_gen_id")]
        public long SpGenId { get; set; }

        [Column("count_of_generation")]
        public long Series { get; set; }
        [Column("avg_phi")]
        public float? AvgPhi {  get; set; }
    }
    /*SELECT number_of_generation AS sp_gen_id,
    count(series) AS count_of_generation,
    avg(phi) AS avg_phi
   FROM sp_gen
  GROUP BY number_of_generation;*/
}
