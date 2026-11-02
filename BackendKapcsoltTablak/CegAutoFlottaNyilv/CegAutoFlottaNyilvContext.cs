using CegAutoFlottaNyilv.Models;
using Microsoft.EntityFrameworkCore;

namespace CegAutoFlottaNyilv;

public class CegAutoFlottaNyilvContext(DbContextOptions<CegAutoFlottaNyilvContext> options) : DbContext(options)
{
    public DbSet<Ember> Ember => Set<Ember>();
    public DbSet<Auto> Auto => Set<Auto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ember>(entity =>
        {
            entity.ToTable("Ember");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Osztaly).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Auto>(entity =>
        {
            entity.ToTable("Auto");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Marka).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Modell).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Rendszam).IsRequired().HasMaxLength(15)
                .UseCollation("Latin1_General_100_CI_AS");
            entity.HasIndex(a => a.Rendszam).IsUnique();
            entity.HasOne(a => a.Ember)
                .WithMany(e => e.Autok)
                .HasForeignKey(a => a.EmberId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
