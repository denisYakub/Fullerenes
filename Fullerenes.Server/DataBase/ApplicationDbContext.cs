using System.Numerics;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fullerenes.Server.DataBase
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<LimitedArea> LimitedAreas { get; set; }
        public DbSet<Fullerene> Fullerenes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            base.OnModelCreating(builder);

            builder.Entity<LimitedArea>()
                .HasDiscriminator<string>("AreaType")
                .HasValue<SphereLimitedArea>("Sphere");

            builder.Entity<LimitedArea>()
                .Property(la => la.Center)
                .HasConversion(
                    center => $"{center.X} {center.Y} {center.Z}",
                    value => Convert(value)
                );

            builder.Entity<Fullerene>()
                .HasOne(la => la.LimitedArea)
                .WithMany(la => la.Fullerenes)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Fullerene>()
                .HasDiscriminator<string>("FullereneType")
                .HasValue<IcosahedronFullerene>("Icosahedron");

            builder.Entity<Fullerene>()
                .Property(fu => fu.Center)
                .HasConversion(
                    center => $"{center.X} {center.Y} {center.Z}",
                    value => Convert(value)
                   );

            builder.Entity<Fullerene>()
                .Property(fu => fu.EulerAngles)
                .HasConversion(
                    eulerAngles => $"{eulerAngles.PraecessioAngle} {eulerAngles.NutatioAngle} {eulerAngles.ProperRotationAngle}",
                    value => new EulerAngles(value)
                );
        }

        public Vector3 Convert(string value)
        {
            var coordinates = value.Split(' ').Select(float.Parse).ToArray();

            float x = 0, y = 0, z = 0;

            if (coordinates.Length == 3)
            {
                x = coordinates[0];
                y = coordinates[1];
                z = coordinates[2];
            }

            return new Vector3(x, y, z);
        }
    }
}
