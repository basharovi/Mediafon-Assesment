using Microsoft.EntityFrameworkCore;
using FileInfo = MediafonTech.ApplicationCore.Entities.FileInfo;

namespace MediafonTech.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<FileInfo> FilesInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Addd the Postgres Extension to support Unique Identifier
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<FileInfo>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnType("uuid");

                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
