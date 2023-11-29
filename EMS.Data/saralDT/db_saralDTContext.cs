using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EMS.Data.saralDT
{
    public partial class db_saralDTContext : DbContext
    {
        public db_saralDTContext()
        {
        }

        public db_saralDTContext(DbContextOptions<db_saralDTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AttDefinition> AttDefinition { get; set; }
        public virtual DbSet<LevAllotment> LevAllotment { get; set; }
        public virtual DbSet<LevDetails> LevDetails { get; set; }
        public virtual DbSet<LevMonthdet> LevMonthdet { get; set; }
        public virtual DbSet<MasEmployee> MasEmployee { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=111.93.52.182,1433;User Id=Leave; Password=Admin@123;Initial Catalog=DT;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<AttDefinition>(entity =>
            {
                entity.HasKey(e => e.Levid);

                entity.ToTable("ATT_DEFINITION");

                entity.Property(e => e.Levid)
                    .HasColumnName("LEVID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Affectsalaryyn).HasColumnName("AFFECTSALARYYN");

                entity.Property(e => e.Allotmentyn).HasColumnName("ALLOTMENTYN");

                entity.Property(e => e.Colimit)
                    .HasColumnName("COLIMIT")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Encashoffset)
                    .HasColumnName("ENCASHOFFSET")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Encashyn).HasColumnName("ENCASHYN");

                entity.Property(e => e.Leaveyn).HasColumnName("LEAVEYN");

                entity.Property(e => e.Levname)
                    .IsRequired()
                    .HasColumnName("LEVNAME")
                    .HasMaxLength(20);

                entity.Property(e => e.Levshort)
                    .IsRequired()
                    .HasColumnName("LEVSHORT")
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<LevAllotment>(entity =>
            {
                entity.HasKey(e => e.Allotid)
                    .HasName("PK_LEAVEALLOTMENT");

                entity.ToTable("LEV_ALLOTMENT");

                entity.Property(e => e.Allotid)
                    .HasColumnName("ALLOTID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Actallotlev)
                    .HasColumnName("ACTALLOTLEV")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Allotfrom)
                    .HasColumnName("ALLOTFROM")
                    .HasColumnType("datetime");

                entity.Property(e => e.Allotlev)
                    .HasColumnName("ALLOTLEV")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Allotto)
                    .HasColumnName("ALLOTTO")
                    .HasColumnType("datetime");

                entity.Property(e => e.Attheadid).HasColumnName("ATTHEADID");

                entity.Property(e => e.Colev)
                    .HasColumnName("COLEV")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Editmodeyn).HasColumnName("EDITMODEYN");

                entity.Property(e => e.Empdetid).HasColumnName("EMPDETID");

                entity.Property(e => e.Empid).HasColumnName("EMPID");

                entity.Property(e => e.Lapse)
                    .HasColumnName("LAPSE")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Levid).HasColumnName("LEVID");
            });

            modelBuilder.Entity<LevDetails>(entity =>
            {
                entity.HasKey(e => new { e.Empid, e.Empdetid, e.Levid, e.Leavedate, e.Firsthalfyn, e.Secondhalfyn })
                    .HasName("PK__LEV_DETA__015EB5C77E2D9D55");

                entity.ToTable("LEV_DETAILS");

                entity.Property(e => e.Empid).HasColumnName("EMPID");

                entity.Property(e => e.Empdetid).HasColumnName("EMPDETID");

                entity.Property(e => e.Levid).HasColumnName("LEVID");

                entity.Property(e => e.Leavedate)
                    .HasColumnName("LEAVEDATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Firsthalfyn).HasColumnName("FIRSTHALFYN");

                entity.Property(e => e.Secondhalfyn).HasColumnName("SECONDHALFYN");

                entity.Property(e => e.Eipyn).HasColumnName("EIPYN");

                entity.Property(e => e.Levmonthly)
                    .HasColumnName("LEVMONTHLY")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Reason)
                    .HasColumnName("REASON")
                    .HasMaxLength(100);

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<LevMonthdet>(entity =>
            {
                entity.HasKey(e => new { e.Empid, e.Empdetid, e.Monthyear })
                    .HasName("PK__LEV_MONT__8F1491F403E676AB");

                entity.ToTable("LEV_MONTHDET");

                entity.Property(e => e.Empid).HasColumnName("EMPID");

                entity.Property(e => e.Empdetid).HasColumnName("EMPDETID");

                entity.Property(e => e.Monthyear).HasColumnName("MONTHYEAR");

                entity.Property(e => e.Adjhrs)
                    .HasColumnName("ADJHRS")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Editmodeyn).HasColumnName("EDITMODEYN");

                entity.Property(e => e.Ndp)
                    .HasColumnName("NDP")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Nod)
                    .HasColumnName("NOD")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Nodcalc)
                    .HasColumnName("NODCALC")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Ot1)
                    .HasColumnName("OT1")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Ot2)
                    .HasColumnName("OT2")
                    .HasColumnType("numeric(15, 2)");

                entity.Property(e => e.Remarks)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MasEmployee>(entity =>
            {
                entity.HasKey(e => e.Empid);

                entity.ToTable("MAS_EMPLOYEE");

                entity.Property(e => e.Empid)
                    .HasColumnName("EMPID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Aadharno)
                    .HasColumnName("AADHARNO")
                    .HasMaxLength(12);

                entity.Property(e => e.Ad1)
                    .HasColumnName("AD1")
                    .HasMaxLength(60);

                entity.Property(e => e.Ad1p)
                    .HasColumnName("AD1P")
                    .HasMaxLength(60);

                entity.Property(e => e.Ad2)
                    .HasColumnName("AD2")
                    .HasMaxLength(60);

                entity.Property(e => e.Ad2p)
                    .HasColumnName("AD2P")
                    .HasMaxLength(60);

                entity.Property(e => e.Ad3)
                    .HasColumnName("AD3")
                    .HasMaxLength(60);

                entity.Property(e => e.Ad3p)
                    .HasColumnName("AD3P")
                    .HasMaxLength(60);

                entity.Property(e => e.Ad4)
                    .HasColumnName("AD4")
                    .HasMaxLength(60);

                entity.Property(e => e.Ad4p)
                    .HasColumnName("AD4P")
                    .HasMaxLength(60);

                entity.Property(e => e.Ad5)
                    .HasColumnName("AD5")
                    .HasMaxLength(60);

                entity.Property(e => e.Ad5p)
                    .HasColumnName("AD5P")
                    .HasMaxLength(60);

                entity.Property(e => e.Adpin)
                    .HasColumnName("ADPIN")
                    .HasMaxLength(7);

                entity.Property(e => e.Adpinp)
                    .HasColumnName("ADPINP")
                    .HasMaxLength(7);

                entity.Property(e => e.Ccmailid)
                    .HasColumnName("CCMAILID")
                    .HasMaxLength(75);

                entity.Property(e => e.Confirmationdate)
                    .HasColumnName("CONFIRMATIONDATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Dircont).HasColumnName("DIRCONT");

                entity.Property(e => e.Directoryn).HasColumnName("DIRECTORYN");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.Docchklist)
                    .HasColumnName("DOCCHKLIST")
                    .HasMaxLength(100);

                entity.Property(e => e.Doj)
                    .HasColumnName("DOJ")
                    .HasColumnType("datetime");

                entity.Property(e => e.Dol)
                    .HasColumnName("DOL")
                    .HasColumnType("datetime");

                entity.Property(e => e.Editmodeyn).HasColumnName("EDITMODEYN");

                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(75);

                entity.Property(e => e.Emaildef).HasColumnName("EMAILDEF");

                entity.Property(e => e.Emailpwd)
                    .HasColumnName("EMAILPWD")
                    .HasMaxLength(25);

                entity.Property(e => e.Emailpwdchgyn).HasColumnName("EMAILPWDCHGYN");

                entity.Property(e => e.Empname)
                    .IsRequired()
                    .HasColumnName("EMPNAME")
                    .HasMaxLength(85);

                entity.Property(e => e.Empremarks)
                    .HasColumnName("EMPREMARKS")
                    .HasMaxLength(255);

                entity.Property(e => e.Emptitle)
                    .HasColumnName("EMPTITLE")
                    .HasMaxLength(6);

                entity.Property(e => e.Esino)
                    .HasColumnName("ESINO")
                    .HasMaxLength(50);

                entity.Property(e => e.Esiward)
                    .HasColumnName("ESIWARD")
                    .HasMaxLength(20);

                entity.Property(e => e.Esiyn).HasColumnName("ESIYN");

                entity.Property(e => e.Ffsyn).HasColumnName("FFSYN");

                entity.Property(e => e.Filteryn).HasColumnName("FILTERYN");

                entity.Property(e => e.Firstname)
                    .HasColumnName("FIRSTNAME")
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.Fname)
                    .HasColumnName("FNAME")
                    .HasMaxLength(50);

                entity.Property(e => e.Idno)
                    .HasColumnName("IDNO")
                    .HasMaxLength(30);

                entity.Property(e => e.Idtype).HasColumnName("IDTYPE");

                entity.Property(e => e.Intrntnlworkeryn).HasColumnName("INTRNTNLWORKERYN");

                entity.Property(e => e.Langemployee)
                    .HasColumnName("LANGEMPLOYEE")
                    .IsUnicode(false);

                entity.Property(e => e.Lastname)
                    .HasColumnName("LASTNAME")
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.Levreason)
                    .HasColumnName("LEVREASON")
                    .HasMaxLength(30);

                entity.Property(e => e.Lmnyear).HasColumnName("LMNYEAR");

                entity.Property(e => e.Lockyn).HasColumnName("LOCKYN");

                entity.Property(e => e.Maritalstatus).HasColumnName("MARITALSTATUS");

                entity.Property(e => e.Marrieddate)
                    .HasColumnName("MARRIEDDATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Midname)
                    .HasColumnName("MIDNAME")
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.Mname)
                    .HasColumnName("MNAME")
                    .HasMaxLength(50);

                entity.Property(e => e.Mobile)
                    .HasColumnName("MOBILE")
                    .HasMaxLength(30);

                entity.Property(e => e.Namefrompan)
                    .HasColumnName("NAMEFROMPAN")
                    .HasMaxLength(100);

                entity.Property(e => e.Npdays).HasColumnName("NPDAYS");

                entity.Property(e => e.Panno)
                    .HasColumnName("PANNO")
                    .HasMaxLength(15);

                entity.Property(e => e.Panward)
                    .HasColumnName("PANWARD")
                    .HasMaxLength(20);

                entity.Property(e => e.Personalemail)
                    .HasColumnName("PERSONALEMAIL")
                    .HasMaxLength(75);

                entity.Property(e => e.Pfbankacno)
                    .HasColumnName("PFBANKACNO")
                    .HasMaxLength(30);

                entity.Property(e => e.Pfdeptno).HasColumnName("PFDEPTNO");

                entity.Property(e => e.Pfifsccode)
                    .HasColumnName("PFIFSCCODE")
                    .HasMaxLength(30);

                entity.Property(e => e.Pfno)
                    .HasColumnName("PFNO")
                    .HasMaxLength(24);

                entity.Property(e => e.Pfrestrictcmpyn).HasColumnName("PFRESTRICTCMPYN");

                entity.Property(e => e.Pfrestrictyn).HasColumnName("PFRESTRICTYN");

                entity.Property(e => e.Pfyn).HasColumnName("PFYN");

                entity.Property(e => e.Pfzeroyn).HasColumnName("PFZEROYN");

                entity.Property(e => e.Phone)
                    .HasColumnName("PHONE")
                    .HasMaxLength(30);

                entity.Property(e => e.Printaddrsyn).HasColumnName("PRINTADDRSYN");

                entity.Property(e => e.Probationdate)
                    .HasColumnName("PROBATIONDATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ptzeroyn).HasColumnName("PTZEROYN");

                entity.Property(e => e.Refno)
                    .IsRequired()
                    .HasColumnName("REFNO")
                    .HasMaxLength(16);

                entity.Property(e => e.Rejempesiyn).HasColumnName("REJEMPESIYN");

                entity.Property(e => e.Rejempid).HasColumnName("REJEMPID");

                entity.Property(e => e.Rejemppfyn).HasColumnName("REJEMPPFYN");

                entity.Property(e => e.Repdoj)
                    .HasColumnName("REPDOJ")
                    .HasColumnType("datetime");

                entity.Property(e => e.Reptyn).HasColumnName("REPTYN");

                entity.Property(e => e.Resignationdate)
                    .HasColumnName("RESIGNATIONDATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Retirementdate)
                    .HasColumnName("RETIREMENTDATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Rflcode)
                    .HasColumnName("RFLCODE")
                    .HasMaxLength(1);

                entity.Property(e => e.Salcalcfrom)
                    .HasColumnName("SALCALCFROM")
                    .HasColumnType("datetime");

                entity.Property(e => e.Sex)
                    .IsRequired()
                    .HasColumnName("SEX")
                    .HasMaxLength(1);

                entity.Property(e => e.Shortname)
                    .HasColumnName("SHORTNAME")
                    .HasMaxLength(25);

                entity.Property(e => e.Spousename)
                    .HasColumnName("SPOUSENAME")
                    .HasMaxLength(50);

                entity.Property(e => e.Ssn)
                    .HasColumnName("SSN")
                    .HasMaxLength(15);

                entity.Property(e => e.Stateid).HasColumnName("STATEID");

                entity.Property(e => e.Stateidp).HasColumnName("STATEIDP");

                entity.Property(e => e.Stdcode)
                    .HasColumnName("STDCODE")
                    .HasMaxLength(7);

                entity.Property(e => e.Tdsedityn).HasColumnName("TDSEDITYN");

                entity.Property(e => e.Tdsyn).HasColumnName("TDSYN");

                entity.Property(e => e.Uanno)
                    .HasColumnName("UANNO")
                    .HasMaxLength(15);
            });
        }
    }
}
