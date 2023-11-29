using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EMS.Website.updateData
{
    public partial class db_dsmanagementnewContext : DbContext
    {
        public db_dsmanagementnewContext()
        {
        }

        public db_dsmanagementnewContext(DbContextOptions<db_dsmanagementnewContext> options)
            : base(options)
        {
        }

        public virtual DbSet<NocMaster> NocMaster { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=192.168.0.252;User Id=usr_dsmanagement; Password=dsmanagement909;Initial Catalog=db_dsmanagementnew;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<NocMaster>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}
