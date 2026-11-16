using ArtMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtMarket.Infrastructure.Persistence;

public class ArtMarketDbContext : DbContext
{
    public ArtMarketDbContext(DbContextOptions<ArtMarketDbContext> options) : base(options)
    {
    }

    public DbSet<ArtworkRecord> Artworks => Set<ArtworkRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ArtworkRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(250);
            entity.Property(e => e.ArtistName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.ArtistTier).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Medium).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ActualPriceUsd).HasColumnType("numeric(18,2)");
            entity.Property(e => e.PredictedPriceUsd).HasColumnType("numeric(18,2)");
        });
    }
    public class ArtMarketDbContextFactory : IDesignTimeDbContextFactory<ArtMarketDbContext>
    {
        public ArtMarketDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ArtMarketDbContext>();

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ArtMarketDb;Username=postgres;Password=s100282s");

            return new ArtMarketDbContext(optionsBuilder.Options);
        }
    }
}
