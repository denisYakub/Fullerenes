using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fullerenes.Server.DataBase
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<SpGen> SpGen { get; set; }
        public DbSet<SpData> SpData { get; set; }
        public DbSet<SpGenGroupView> SpGenGroupView { get; set; }
        public DbSet<SpGenIdCounter> SpGenIdCounter { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SpGenGroupView>((vw =>
            {
                vw.HasNoKey();
                vw.ToView("gen_group_view");
            }));
        }
    }
}
