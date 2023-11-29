using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EMS.Data.Models
{
    public partial class stagingems_10Jan23Context : DbContext
    {
        public stagingems_10Jan23Context()
        {
        }

        public stagingems_10Jan23Context(DbContextOptions<stagingems_10Jan23Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Sme> Sme { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=111.93.53.168;Database=stagingems_10Jan23;User Id=stagingems_10Jan23;Password=fgrfgfdgdrh;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Sme>(entity =>
            {
                entity.ToTable("SME");

                entity.HasIndex(e => e.Id)
                    .HasName("IX_SME");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Level1).HasColumnName("Level_1");

                entity.Property(e => e.Level2).HasColumnName("Level_2");

                entity.Property(e => e.Level3).HasColumnName("Level_3");

                entity.Property(e => e.Level4).HasColumnName("Level_4");

                entity.Property(e => e.Level5).HasColumnName("Level_5");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.SubjectMatterExpert)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.HasSequence<int>("Sq")
                .HasMin(1)
                .IsCyclic();
        }
    }
}
