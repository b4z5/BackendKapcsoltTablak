using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CegAutoFlottaNyilv.Migrations;

[DbContext(typeof(CegAutoFlottaNyilvContext))]
partial class CegAutoFlottaNyilvContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.21")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("CegAutoFlottaNyilv.Models.Auto", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
            b.Property<int?>("EmberId").HasColumnType("int");
            b.Property<int>("Evjarat").HasColumnType("int");
            b.Property<string>("Marka").IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");
            b.Property<string>("Modell").IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");
            b.Property<string>("Rendszam").IsRequired().HasMaxLength(15)
                .HasColumnType("nvarchar(15)")
                .UseCollation("Latin1_General_100_CI_AS");
            b.HasKey("Id");
            b.HasIndex("EmberId");
            b.HasIndex("Rendszam").IsUnique();
            b.ToTable("Auto");
        });

        modelBuilder.Entity("CegAutoFlottaNyilv.Models.Ember", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
            b.Property<string>("Nev").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
            b.Property<string>("Osztaly").IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");
            b.HasKey("Id");
            b.ToTable("Ember");
        });

        modelBuilder.Entity("CegAutoFlottaNyilv.Models.Auto", b =>
        {
            b.HasOne("CegAutoFlottaNyilv.Models.Ember", "Ember")
                .WithMany("Autok")
                .HasForeignKey("EmberId")
                .OnDelete(DeleteBehavior.SetNull);
            b.Navigation("Ember");
        });

        modelBuilder.Entity("CegAutoFlottaNyilv.Models.Ember", b =>
        {
            b.Navigation("Autok");
        });
    }
}
