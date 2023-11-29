using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EMS.Data.Models
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

        public virtual DbSet<Designation> Designation { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }

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

            modelBuilder.Entity<Designation>(entity =>
            {
                entity.Property(e => e.Experience).HasMaxLength(200);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.HasIndex(e => e.UserName)
                    .HasName("uc_UserLogin")
                    .IsUnique();

                entity.Property(e => e.AadharNumber).HasMaxLength(20);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Address).HasColumnType("ntext");

                entity.Property(e => e.AlternativeNumber).HasMaxLength(50);

                entity.Property(e => e.ApiPassword).HasMaxLength(50);

                entity.Property(e => e.CrmuserId).HasColumnName("CRMUserId");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.EmailOffice).HasMaxLength(200);

                entity.Property(e => e.EmailPersonal).HasMaxLength(200);

                entity.Property(e => e.EmpCode).HasMaxLength(50);

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Hrmid).HasColumnName("HRMId");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsFromDbdt).HasColumnName("IsFromDBDT");

                entity.Property(e => e.IsInterestedPffaccount).HasColumnName("IsInterestedPFFAccount");

                entity.Property(e => e.IsSpeg).HasColumnName("IsSPEG");

                entity.Property(e => e.JobTitle).HasMaxLength(200);

                entity.Property(e => e.JoinedDate).HasColumnType("datetime");

                entity.Property(e => e.MarraigeDate).HasColumnType("date");

                entity.Property(e => e.MobileNumber).HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.OtherTechnology).HasMaxLength(250);

                entity.Property(e => e.PanNumber).HasMaxLength(20);

                entity.Property(e => e.PassportNumber).HasMaxLength(20);

                entity.Property(e => e.PasswordKey).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.Pmuid).HasColumnName("PMUid");

                entity.Property(e => e.ProfilePicture)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.RelievingDate).HasColumnType("datetime");

                entity.Property(e => e.ResignationDate).HasColumnType("datetime");

                entity.Property(e => e.SkypeId).HasMaxLength(200);

                entity.Property(e => e.Title).HasMaxLength(20);

                entity.Property(e => e.Tlid).HasColumnName("TLId");

                entity.Property(e => e.Uannumber)
                    .HasColumnName("UANNumber")
                    .HasMaxLength(30);

                entity.Property(e => e.UserName).HasMaxLength(200);

                entity.HasOne(d => d.Designation)
                    .WithMany(p => p.UserLogin)
                    .HasForeignKey(d => d.DesignationId)
                    .HasConstraintName("FK_UserLogin_Designation");

                entity.HasOne(d => d.Pmu)
                    .WithMany(p => p.InversePmu)
                    .HasForeignKey(d => d.Pmuid)
                    .HasConstraintName("FK__PMUid__UID");
            });
        }
    }
}
