using Konyvtar.Modellek;
using Microsoft.EntityFrameworkCore;

namespace Konyvtar.Adatok;

public class KonyvtarContext(DbContextOptions<KonyvtarContext> options) : DbContext(options)
{
    public DbSet<Olvaso> Olvasok => Set<Olvaso>();
    public DbSet<Konyv> Konyvek => Set<Konyv>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Olvaso>(entity =>
        {
            entity.ToTable("Olvasok");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Nev).IsRequired().HasMaxLength(100);
            entity.Property(o => o.Email).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Konyv>(entity =>
        {
            entity.ToTable("Konyvek");
            entity.HasKey(k => k.Id);
            entity.Property(k => k.Cim).IsRequired().HasMaxLength(150);
            entity.Property(k => k.Szerzo).IsRequired().HasMaxLength(100);
            entity.Property(k => k.ISBN).IsRequired().HasMaxLength(20).UseCollation("NOCASE");
            entity.HasIndex(k => k.ISBN).IsUnique();
            entity.HasOne(k => k.Olvaso)
                .WithMany(o => o.Konyvek)
                .HasForeignKey(k => k.OlvasoId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
