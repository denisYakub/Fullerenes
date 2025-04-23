using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fullerenes.Server.DataBase
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<SpGen> SpGen { get; set; }
        public DbSet<SpData> SpData { get; set; }
        public DbSet<SpGenGroup> SpGenGroup { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SpGenGroup>((vw =>
            {
                vw.HasNoKey();
                vw.ToView("Gen_Group_View");
            }));
        }
    }
}
