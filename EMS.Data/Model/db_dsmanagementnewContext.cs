using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EMS.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EMS.Data
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

        public virtual DbSet<Sme> Sme { get; set; }
        public virtual DbSet<Wfhactivity> Wfhactivity { get; set; }
        public virtual DbSet<WfhtypesMaster> WfhtypesMaster { get; set; }

        public virtual DbSet<Reminder> Reminder { get; set; }
        public virtual DbSet<ReminderUser> ReminderUser { get; set; }
        public virtual DbSet<TeamHierarchy> TeamHierarchy { get; set; }
        public virtual DbSet<EstimateHourFileNameType> EstimateHourFileNameType { get; set; }
        public virtual DbSet<EstimateHour> EstimateHour { get; set; }
        public virtual DbSet<EstimateHourParentTechnology> EstimateHourParentTechnology { get; set; }
        public virtual DbSet<EstimateHourTechnology> EstimateHourTechnology { get; set; }
        public virtual DbSet<ProjectClosureAbroadPm> ProjectClosureAbroadPm { get; set; }
        public virtual DbSet<LibraryTemplate> LibraryTemplate { get; set; }
        public virtual DbSet<LibraryTemplateType> LibraryTemplateType { get; set; }
        public virtual DbSet<SerialNumber> SerialNumber { get; set; }
        public virtual DbSet<TechnologyParent> TechnologyParent { get; set; }
        public virtual DbSet<TechnologyParentMapping> TechnologyParentMapping { get; set; }
        public virtual DbSet<LibraryComponent> LibraryComponent { get; set; }
        public virtual DbSet<LibraryComponentFile> LibraryComponentFile { get; set; }
        public virtual DbSet<LibraryComponentType> LibraryComponentType { get; set; }
        public virtual DbSet<AbroadPM> AbroadPm { get; set; }
        public virtual DbSet<Accessory> Accessory { get; set; }
        public virtual DbSet<AlertMessage> AlertMessage { get; set; }
        public virtual DbSet<AlertMessageRead> AlertMessageRead { get; set; }
        public virtual DbSet<Appraisal> Appraisal { get; set; }
        public virtual DbSet<AppraisalExtras> AppraisalExtras { get; set; }
        public virtual DbSet<AvailUser> AvailUser { get; set; }
        public virtual DbSet<BloodGroup> BloodGroup { get; set; }
        public virtual DbSet<BucketModel> BucketModel { get; set; }
        //public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<CadidateExam> CadidateExam { get; set; }
        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<CandidateAnswer> CandidateAnswer { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Communication> Communication { get; set; }
        public virtual DbSet<CompanyDocument> CompanyDocument { get; set; }
        public virtual DbSet<CompanyOffice> CompanyOffice { get; set; }
        public virtual DbSet<Complaint> Complaint { get; set; }
        public virtual DbSet<ComplaintUser> ComplaintUser { get; set; }
        public virtual DbSet<Component> Component { get; set; }
        public virtual DbSet<ComponentCategory> ComponentCategory { get; set; }
        public virtual DbSet<ConferenceRoom> ConferenceRoom { get; set; }
        public virtual DbSet<ConferenceRoomBooking> ConferenceRoomBooking { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<CurrentOpening> CurrentOpening { get; set; }
        public virtual DbSet<DailyThought> DailyThought { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DesignerManagement> DesignerManagement { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceAccessoriesMap> DeviceAccessoriesMap { get; set; }
        public virtual DbSet<DeviceCategory> DeviceCategory { get; set; }
        public virtual DbSet<DeviceDetail> DeviceDetail { get; set; }
        public virtual DbSet<DeviceDeviceHistory> DeviceDeviceHistory { get; set; }
        public virtual DbSet<DeviceDeviceInfo> DeviceDeviceInfo { get; set; }
        public virtual DbSet<DomainType> DomainType { get; set; }
        public virtual DbSet<ElanceAssignedJob> ElanceAssignedJob { get; set; }
        public virtual DbSet<ElanceCredential> ElanceCredential { get; set; }
        public virtual DbSet<ElanceJobDetails> ElanceJobDetails { get; set; }
        public virtual DbSet<EmpLateActivity> EmpLateActivity { get; set; }
        public virtual DbSet<EmployeeActivity> EmployeeActivity { get; set; }
        public virtual DbSet<EmployeeAnswers> EmployeeAnswers { get; set; }
        public virtual DbSet<EmployeeAppraise> EmployeeAppraise { get; set; }
        public virtual DbSet<EmployeeComplaint> EmployeeComplaint { get; set; }
        public virtual DbSet<EmployeeFeedback> EmployeeFeedback { get; set; }
        public virtual DbSet<EmployeeFeedbackRank> EmployeeFeedbackRank { get; set; }
        public virtual DbSet<EmployeeFeedbackRankStatus> EmployeeFeedbackRankStatus { get; set; }
        public virtual DbSet<EmployeeFeedbackReason> EmployeeFeedbackReason { get; set; }
        public virtual DbSet<EmployeeFeedbackReasonMapping> EmployeeFeedbackReasonMapping { get; set; }
        public virtual DbSet<EmployeeMedicalData> EmployeeMedicalData { get; set; }
        public virtual DbSet<EmployeeProject> EmployeeProject { get; set; }
        public virtual DbSet<EmployeeRelativeMedicalData> EmployeeRelativeMedicalData { get; set; }
        public virtual DbSet<Escalation> Escalation { get; set; }
        public virtual DbSet<EscalationConclusion> EscalationConclusion { get; set; }
        public virtual DbSet<EscalationDocument> EscalationDocument { get; set; }
        public virtual DbSet<EscalationRootCause> EscalationRootCause { get; set; }
        public virtual DbSet<EscalationType> EscalationType { get; set; }
        public virtual DbSet<EsculationForUser> EsculationForUser { get; set; }
        public virtual DbSet<EsculationFoundForUser> EsculationFoundForUser { get; set; }
        public virtual DbSet<EstimateCountry> EstimateCountry { get; set; }
        public virtual DbSet<EstimateDocument> EstimateDocument { get; set; }
        public virtual DbSet<EstimateTechnology> EstimateTechnology { get; set; }
        public virtual DbSet<EstimateHostingPackage> EstimateHostingPackage { get; set; }
        public virtual DbSet<EstimateHostingPackageTechnology> EstimateHostingPackageTechnology { get; set; }
        public virtual DbSet<EstimateModel> EstimateModel { get; set; }
        public virtual DbSet<Examination> Examination { get; set; }
        public virtual DbSet<ExamQuestion> ExamQuestion { get; set; }
        public virtual DbSet<ExamQuestionAnswerDetail> ExamQuestionAnswerDetail { get; set; }
        public virtual DbSet<ExamQuestionDetail> ExamQuestionDetail { get; set; }
        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<FinancialYear> FinancialYear { get; set; }
        public virtual DbSet<Forecasting> Forecasting { get; set; }
        public virtual DbSet<ForecastingDepartment> ForecastingDepartment { get; set; }
        public virtual DbSet<ForumFeedback> ForumFeedback { get; set; }
        public virtual DbSet<Forums> Forums { get; set; }
        public virtual DbSet<FrontMenu> FrontMenu { get; set; }
        public virtual DbSet<FullLeave> FullLeave { get; set; }
        public virtual DbSet<HalfLeave> HalfLeave { get; set; }
        public virtual DbSet<IntwExperience> IntwExperience { get; set; }
        public virtual DbSet<IntwQues> IntwQues { get; set; }
        public virtual DbSet<IntwQuesExp> IntwQuesExp { get; set; }
        public virtual DbSet<IntwQuestype> IntwQuestype { get; set; }
        public virtual DbSet<IntwQusOptions> IntwQusOptions { get; set; }
        public virtual DbSet<IntwTechnology> IntwTechnology { get; set; }
        public virtual DbSet<IntwUser> IntwUser { get; set; }
        public virtual DbSet<IntwUserAnswer> IntwUserAnswer { get; set; }
        public virtual DbSet<IntwUserQues> IntwUserQues { get; set; }
        public virtual DbSet<IntwUserSession> IntwUserSession { get; set; }
        public virtual DbSet<Investment> Investment { get; set; }
        public virtual DbSet<InvestmentDocument> InvestmentDocument { get; set; }
        public virtual DbSet<InvestmentMonth> InvestmentMonth { get; set; }
        public virtual DbSet<InvestmentType> InvestmentType { get; set; }
        public virtual DbSet<InvestmentTypeAmountMap> InvestmentTypeAmountMap { get; set; }
        public virtual DbSet<JobReference> JobReference { get; set; }
        public virtual DbSet<KnowledgeBase> KnowledgeBase { get; set; }
        public virtual DbSet<KnowledgeDepartment> KnowledgeDepartment { get; set; }
        public virtual DbSet<KnowledgeTech> KnowledgeTech { get; set; }
        public virtual DbSet<LateHour> LateHour { get; set; }
        public virtual DbSet<LeadClient> LeadClient { get; set; }

        public virtual DbSet<Leadership> Leadership { get; set; }
        public virtual DbSet<LeadStatu> LeadStatus { get; set; }
        public virtual DbSet<LeadTechnician> LeadTechnician { get; set; }
        public virtual DbSet<LeadTechnicianArchive> LeadTechnicianArchive { get; set; }
        public virtual DbSet<LeadTransaction> LeadTransaction { get; set; }
        public virtual DbSet<LeadTransactionArchive> LeadTransactionArchive { get; set; }
        public virtual DbSet<LeaveActivity> LeaveActivity { get; set; }
        public virtual DbSet<LeaveTypesMaster> LeaveTypesMaster { get; set; }
        public virtual DbSet<LeaveActivityAdjust> LeaveActivityAdjust { get; set; }
        public virtual DbSet<LeaveAdjust> LeaveAdjust { get; set; }
        public virtual DbSet<Management> Management { get; set; }
        public virtual DbSet<MeetingMaster> MeetingMaster { get; set; }
        public virtual DbSet<MeetingMinutes> MeetingMinutes { get; set; }
        public virtual DbSet<MenuAccess> MenuAccess { get; set; }
        public virtual DbSet<MomMeeting> MomMeeting { get; set; }
        public virtual DbSet<MomMeetingDepartment> MomMeetingDepartment { get; set; }
        public virtual DbSet<MomMeetingParticipant> MomMeetingParticipant { get; set; }
        public virtual DbSet<MomMeetingTask> MomMeetingTask { get; set; }
        public virtual DbSet<MomMeetingTaskParticipant> MomMeetingTaskParticipant { get; set; }
        public virtual DbSet<MomMeetingTaskTimeLine> MomMeetingTaskTimeLine { get; set; }
        public virtual DbSet<OfficialLeave> OfficialLeave { get; set; }
        public virtual DbSet<OrgDocument> OrgDocument { get; set; }
        public virtual DbSet<OrgDocumentApprove> OrgDocumentApprove { get; set; }
        public virtual DbSet<OrgDocumentDepartment> OrgDocumentDepartment { get; set; }
        public virtual DbSet<OrgDocumentMaster> OrgDocumentMaster { get; set; }
        public virtual DbSet<OrgDocumentRole> OrgDocumentRole { get; set; }
        public virtual DbSet<PersonalDevelopment> PersonalDevelopment { get; set; }
        public virtual DbSet<PfreviewQuarter> PfreviewQuarter { get; set; }
        public virtual DbSet<PfreviewQuestion> PfreviewQuestion { get; set; }
        public virtual DbSet<PfreviewResult> PfreviewResult { get; set; }
        public virtual DbSet<PfreviewSubmitted> PfreviewSubmitted { get; set; }
        public virtual DbSet<PILog> Pilog { get; set; }
        public virtual DbSet<Portfolio> Portfolio { get; set; }
        public virtual DbSet<PortfolioDomain> PortfolioDomain { get; set; }
        public virtual DbSet<PortfolioTech> PortfolioTech { get; set; }
        public virtual DbSet<Preference> Preferences { get; set; }
        public virtual DbSet<Productivity> Productivity { get; set; }
        public virtual DbSet<ProductLanding> ProductLanding { get; set; }
        public virtual DbSet<ProductLandingScreenshot> ProductLandingScreenshot { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectAdditionalSupport> ProjectAdditionalSupport { get; set; }
        public virtual DbSet<ProjectAdditionalSupportUser> ProjectAdditionalSupportUser { get; set; }
        public virtual DbSet<ProjectAuditPA> ProjectAuditPa { get; set; }
        public virtual DbSet<ProjectClose> ProjectClose { get; set; }
        public virtual DbSet<ProjectClosure> ProjectClosure { get; set; }
        public virtual DbSet<ProjectClosureDetail> ProjectClosureDetail { get; set; }
        public virtual DbSet<ProjectClosureReview> ProjectClosureReview { get; set; }
        public virtual DbSet<Project_Department> ProjectDepartment { get; set; }
        public virtual DbSet<ProjectDeveloper> ProjectDeveloper { get; set; }
        public virtual DbSet<ProjectDeveloperAddon> ProjectDeveloperAddon { get; set; }
        public virtual DbSet<ProjectInvoice> ProjectInvoice { get; set; }
        public virtual DbSet<ProjectInvoiceComment> ProjectInvoiceComment { get; set; }
        public virtual DbSet<ProjectLead> ProjectLead { get; set; }
        public virtual DbSet<ProjectLeadArchive> ProjectLeadArchive { get; set; }
        public virtual DbSet<ProjectLeadTech> ProjectLeadTech { get; set; }
        public virtual DbSet<ProjectLeadTechArchive> ProjectLeadTechArchive { get; set; }
        public virtual DbSet<ProjectLesson> ProjectLesson { get; set; }
        public virtual DbSet<ProjectLessonLearned> ProjectLessonLearned { get; set; }
        public virtual DbSet<ProjectLessonTopic> ProjectLessonTopic { get; set; }
        public virtual DbSet<ProjectNCLog> ProjectNclog { get; set; }
        public virtual DbSet<ProjectPm> ProjectPm { get; set; }
        public virtual DbSet<Project_Tech> ProjectTech { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        public virtual DbSet<ReadMessage> ReadMessage { get; set; }
        public virtual DbSet<Relationship> Relationship { get; set; }
        public virtual DbSet<ReportBug> ReportBug { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<SaturdayManagement> SaturdayManagement { get; set; }
        public virtual DbSet<ServiceAuth> ServiceAuth { get; set; }
        public virtual DbSet<Sim> Sim { get; set; }
        public virtual DbSet<SubProject> SubProjects { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<TaskAssignedTo> TaskAssignedTo { get; set; }
        public virtual DbSet<TaskComment> TaskComment { get; set; }
        public virtual DbSet<TaskStatu> TaskStatus { get; set; }
        public virtual DbSet<Technology> Technology { get; set; }
        public virtual DbSet<TypeMaster> TypeMaster { get; set; }
        public virtual DbSet<TdsassesmentYear> TdsassesmentYear { get; set; }
        public virtual DbSet<TdsdeductionDoc> TdsdeductionDoc { get; set; }
        public virtual DbSet<TdsdeductionType> TdsdeductionType { get; set; }
        public virtual DbSet<TdsempDeduction> TdsempDeduction { get; set; }
        public virtual DbSet<Tdstype> Tdstype { get; set; }

        public virtual DbSet<UserActivity> UserActivity { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLog { get; set; }
        public virtual DbSet<UserLog> UserLog { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }
        public virtual DbSet<VaccinationStatus> VaccinationStatus { get; set; }
        public virtual DbSet<Vaccine> Vaccine { get; set; }
        public virtual DbSet<User_Tech> UserTech { get; set; }
        public virtual DbSet<UserTimeSheet> UserTimeSheet { get; set; }
        public virtual DbSet<VirtualDeveloper> VirtualDeveloper { get; set; }
        public virtual DbSet<ProjectOtherPm> ProjectOtherPm { get; set; }
        public virtual DbSet<DomainExperts> DomainExperts { get; set; }
        public virtual DbSet<UserExitProcess> UserExitProcess { get; set; }
        public virtual DbSet<NocMaster> NocMaster { get; set; }
        public virtual DbSet<UserNoc> UserNoc { get; set; }
        public virtual DbSet<Library> Library { get; set; }
        public virtual DbSet<LibraryFile> LibraryFile { get; set; }
        public virtual DbSet<LibraryFileType> LibraryFileType { get; set; }
        public virtual DbSet<LibraryIndustry> LibraryIndustry { get; set; }
        public virtual DbSet<LibraryLayoutType> LibraryLayoutType { get; set; }
        public virtual DbSet<LibraryLayoutTypeMapping> LibraryLayoutTypeMapping { get; set; }
        public virtual DbSet<LibraryTechnology> LibraryTechnology { get; set; }
        public virtual DbSet<LibrarySearchLayoutTypeMapping> LibrarySearchLayoutTypeMapping { get; set; }
        public virtual DbSet<LibrarySearchComponent> LibrarySearchComponent { get; set; }
        public virtual DbSet<OrgImprovement> OrgImprovement { get; set; }
        public virtual DbSet<EstimateDocumentIndustry> EstimateDocumentIndustry { get; set; }
        public virtual DbSet<ProjectLeadIndustry> ProjectLeadIndustry { get; set; }
        public virtual DbSet<LessonLearnt> LessonLearnt { get; set; }
        public virtual DbSet<UserActivityManageProject> UserActivityManageProject { get; set; }
        public virtual DbSet<EstimateRole> EstimateRole { get; set; }
        public virtual DbSet<EstimateRoleExp> EstimateRoleExp { get; set; }
        public virtual DbSet<EstimateRoleTechnoloyPrice> EstimateRoleTechnoloyPrice { get; set; }
        public virtual DbSet<EstimatePriceCalculation> EstimatePriceCalculation { get; set; }
        public virtual DbSet<EstimatePriceCalculationDetail> EstimatePriceCalculationDetail { get; set; }
        public virtual DbSet<UserDocument> UserDocument { get; set; }

        public virtual DbSet<SalesKitType> SalesKitType { get; set; }
        public virtual DbSet<CvsType> CvsType { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentDepartment> DocumentDepartment { get; set; }

        public virtual DbSet<Designation> Designation { get; set; }
        public virtual DbSet<RoleCategory> RoleCategory { get; set; }

        public virtual DbSet<CvBuilderEducation> CvBuilderEducation { get; set; }
        public virtual DbSet<Cvbuilder> Cvbuilder { get; set; }
        public virtual DbSet<CvbuilderCertifications> CvbuilderCertifications { get; set; }
        public virtual DbSet<CvbuilderCoreCompetencies> CvbuilderCoreCompetencies { get; set; }
        public virtual DbSet<CvbuilderPreviousExperience> CvbuilderPreviousExperience { get; set; }
        public virtual DbSet<ExperienceType> ExperienceType { get; set; }
        public virtual DbSet<CvbuilderIndustry> CvbuilderIndustry { get; set; }
        public virtual DbSet<CvbuilderTechnology> CvbuilderTechnology { get; set; }
        public virtual DbSet<CvBuilderEstimatePrice> CvBuilderEstimatePrice { get; set; }
        public virtual DbSet<StudyDocumentFiles> StudyDocumentFiles { get; set; }
        public virtual DbSet<StudyDocuments> StudyDocuments { get; set; }
        public virtual DbSet<StudyDocumentsPermissions> StudyDocumentsPermissions { get; set; }
        public virtual DbSet<RequestedStudyDocuments> RequestedStudyDocuments { get; set; }
        public virtual DbSet<DocumentLibrary> DocumentLibrary { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=192.168.0.252;Database=db_dsmanagementnew;user id=usr_dsmanagement;password=dsmanagement909;");

            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

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

            modelBuilder.Entity<Kra>(entity =>
            {
                entity.ToTable("KRA");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("Description")
                    .HasMaxLength(50);

                entity.Property(e => e.Title)
                    .HasColumnName("Title")
                    .HasMaxLength(50);
            });



            modelBuilder.Entity<Designation>(entity =>
            {
                entity.Property(e => e.Experience).HasMaxLength(200);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(200);
            });


            modelBuilder.Entity<RoleCategory>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<TechnologyParent>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TechnologyParentMapping>(entity =>
            {
                entity.HasKey(e => new { e.TechnologyId, e.TechnologyParentId });

                entity.HasOne(d => d.Technology)
                    .WithMany(p => p.TechnologyParentMapping)
                    .HasForeignKey(d => d.TechnologyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TechnologyParent)
                    .WithMany(p => p.TechnologyParentMapping)
                    .HasForeignKey(d => d.TechnologyParentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TechnologyParentMapping_Technology_TechnologyParentId");
            });

            modelBuilder.Entity<LibraryComponent>(entity =>
            {
                entity.HasKey(e => new { e.LibraryId, e.LibraryComponentTypeId });
            });

            modelBuilder.Entity<LibraryComponentFile>(entity =>
            {
                entity.HasIndex(e => e.SrNo)
                    .HasName("UQ__LibraryC__C3A4D3CD4FBD9286")
                    .IsUnique();

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FileId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.KeyId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.PsdfilePath)
                    .HasColumnName("PSDFilePath")
                    .HasMaxLength(1000);

                entity.Property(e => e.SrNo).HasColumnName("SrNO");
                entity.Property(e => e.LibraryComponentFileTypeId).HasColumnName("LibraryComponentFileTypeId");

                entity.HasOne(d => d.LibraryFileType)
                    .WithMany(p => p.LibraryComponentFile)
                    .HasForeignKey(d => d.LibraryComponentFileTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LibraryComponentFile_LibraryFileType_LibraryComponentFileTypeId");

                entity.HasOne(d => d.Library)
                    .WithMany(p => p.LibraryComponentFile)
                    .HasForeignKey(d => d.LibraryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LibraryComponentType>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(300)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<DomainExperts>(entity =>
            {
                entity.HasKey(e => new { e.DomainId, e.Uid })
                    .HasName("PK__DomainEx__18C3CE36008AE713");

                entity.Property(e => e.Uid).HasColumnName("UId");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.DomainExperts)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DomainExper__UId__036753BE");
            });

            modelBuilder.Entity<VaccinationStatus>(entity =>
            {
                entity.Property(e => e.Uid).HasColumnName("UId");

                entity.Property(e => e.UpdatedCertificate).HasMaxLength(1000);
                entity.Property(e => e.DeclarationName).HasMaxLength(200);

                entity.HasOne(d => d.U)
                    .WithMany(p => p.VaccinationStatus)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_VaccinationStatus_UserLogin_UID");

                entity.HasOne(d => d.VaccinationType)
                    .WithMany(p => p.VaccinationStatus)
                    .HasForeignKey(d => d.VaccinationTypeId);
            });

            modelBuilder.Entity<Vaccine>(entity =>
            {
                entity.Property(e => e.VaccineName)
                    .IsRequired()
                    .HasMaxLength(200);
            });
            modelBuilder.Entity<AbroadPM>(entity =>
            {
                entity.HasKey(e => e.AutoID);

                entity.ToTable("AbroadPM");

                entity.Property(e => e.AutoID).HasColumnName("AutoID");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.isActive).HasColumnName("isActive");
                entity.Property(e => e.isDefaultForEmail).HasColumnName("isDefaultForEmail");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.AbroadPM)
                    .HasForeignKey(d => d.Uid);
            });

            modelBuilder.Entity<ProjectClosureAbroadPm>(entity =>
            {
                entity.HasKey(e => new { e.ProjectClosureId, e.AbroadPmid });

                entity.ToTable("ProjectClosureAbroadPM");

                entity.Property(e => e.AbroadPmid).HasColumnName("AbroadPMId");

                entity.HasOne(d => d.AbroadPm)
                    .WithMany(p => p.ProjectClosureAbroadPm)
                    .HasForeignKey(d => d.AbroadPmid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectClosureAbroadPM_AbroadPM_AutoID");

                entity.HasOne(d => d.ProjectClosure)
                    .WithMany(p => p.ProjectClosureAbroadPm)
                    .HasForeignKey(d => d.ProjectClosureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectClosureAbroadPM_ProjectClosure_Id");
            });

            modelBuilder.Entity<Accessory>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.CreateByU)
                    .WithMany(p => p.AccessoryCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Accessory_UserLogin_CreatedByUid");

                entity.HasOne(d => d.ModifyByU)
                    .WithMany(p => p.AccessoryModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AlertMessage>(entity =>
            {
                entity.HasKey(e => e.AlertId);

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.AlertMessage)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AlertMessage_UserLogin");
            });

            modelBuilder.Entity<AlertMessageRead>(entity =>
            {
                entity.HasKey(e => e.AlertReadId);

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.HasOne(d => d.Alert)
                    .WithMany(p => p.AlertMessageRead)
                    .HasForeignKey(d => d.AlertId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AlertMessageRead_AlertMessage");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.AlertMessageRead)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AlertMessageRead_UserLogin");
            });

            modelBuilder.Entity<Appraisal>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.IsCommit).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsCommitTl)
                    .HasColumnName("IsCommitTL")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PeriodEnd).HasColumnType("date");

                entity.Property(e => e.PeriodStart).HasColumnType("date");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Appraisal)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appraisal_UserLogin");
            });

            modelBuilder.Entity<AppraisalExtras>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Comments).HasMaxLength(1000);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.Tlcomments)
                    .HasColumnName("TLComments")
                    .HasMaxLength(1000);

                entity.Property(e => e.TluserId).HasColumnName("TLUserId");

                entity.HasOne(d => d.Appraisal)
                    .WithMany(p => p.AppraisalExtras)
                    .HasForeignKey(d => d.AppraisalId)
                    .HasConstraintName("FK_AppraisalExtras_Appraisal");
            });

            modelBuilder.Entity<AvailUser>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModify)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.Property(e => e.IsCurrent)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.AvailUserU)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AvailUser__UID__4B2D1C3C");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AvailUserUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AvailUser__UserI__4C214075");
            });

            modelBuilder.Entity<BloodGroup>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<BucketModel>(entity =>
            {
                entity.HasKey(e => e.BucketId);

                entity.HasIndex(e => e.ModelCode)
                    .HasName("unique_modelcode")
                    .IsUnique();

                entity.HasIndex(e => e.ModelName)
                    .HasName("IX_BucketModel")
                    .IsUnique();

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.IP)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.ModelCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.ModelName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CadidateExam>(entity =>
            {
                entity.HasKey(e => e.CexamId);

                entity.Property(e => e.CexamId).HasColumnName("CExamID");

                entity.Property(e => e.CandidateId).HasColumnName("CandidateID");

                entity.Property(e => e.DateOfExam).HasColumnType("date");

                entity.Property(e => e.ExamId).HasColumnName("ExamID");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.HasOne(d => d.Candidate)
                    .WithMany(p => p.CadidateExam)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CadidateE__Candi__3631FF56");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.CadidateExam)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CadidateE__ExamI__3726238F");
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("uc_Candidate")
                    .IsUnique();

                entity.Property(e => e.CandidateId).HasColumnName("CandidateID");

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Contact)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CandidateAnswer>(entity =>
            {
                entity.HasKey(e => e.CanswerId);

                entity.Property(e => e.CanswerId).HasColumnName("CAnswerID");

                entity.Property(e => e.Canswer)
                    .HasColumnName("CAnswer")
                    .HasMaxLength(2000);

                entity.Property(e => e.CquestionId).HasColumnName("CQuestionId");

                entity.HasOne(d => d.Cquestion)
                    .WithMany(p => p.CandidateAnswer)
                    .HasForeignKey(d => d.CquestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Candidate__CQues__3DD3211E");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Address).HasColumnType("ntext");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Msn)
                    .HasColumnName("MSN")
                    .HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.PMUid).HasColumnName("PMUid");
            });

            modelBuilder.Entity<Communication>(entity =>
            {
                entity.HasKey(e => e.Cid);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<CompanyDocument>(entity =>
            {
                entity.Property(e => e.DocumentName).HasMaxLength(250);

                entity.Property(e => e.DocumentPath).HasMaxLength(500);

                entity.Property(e => e.Heading).HasMaxLength(250);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.CompanyDocument)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK__CompanyDo__Depar__530E3526");
            });

            modelBuilder.Entity<CompanyOffice>(entity =>
            {
                entity.Property(e => e.Country)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OfficeAddress)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ClientComplainDate).HasColumnType("datetime");

                entity.Property(e => e.DeveloperComplainDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.TlComplainDate).HasColumnType("datetime");

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.Complaint)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Complaint_UserLogin");
            });
            modelBuilder.Entity<ComplaintUser>(entity =>
            {
                entity.HasKey(e => new { e.UserLoginId, e.ComplaintId })
                    .HasName("PK__Complain__F73D8E1416451E08");

                entity.HasOne(d => d.Complaint)
                    .WithMany(p => p.ComplaintUser)
                    .HasForeignKey(d => d.ComplaintId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Complaint__Compl__19218AB3");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ComplaintUser)
                    .HasForeignKey(d => d.UserLoginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Complaint__UserL__182D667A");
            });

            modelBuilder.Entity<Component>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DesignImages).HasMaxLength(200);

                entity.Property(e => e.ImageName).HasMaxLength(250);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.PsdImages).HasMaxLength(200);

                entity.Property(e => e.Tags).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(250);

                entity.HasOne(d => d.ComponentCategory)
                    .WithMany(p => p.Component)
                    .HasForeignKey(d => d.ComponentCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Component__Compo__5B6E70FD");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.Component)
                    .HasForeignKey(d => d.CreatedByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Component__Creat__5C629536");
            });

            modelBuilder.Entity<ComponentCategory>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(250);
            });

            modelBuilder.Entity<ConferenceRoom>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ThemeColor)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.CompanyOffice)
                    .WithMany(p => p.ConferenceRoom)
                    .HasForeignKey(d => d.CompanyOfficeId)
                    .HasConstraintName("FK__Conferenc__Compa__3ACC9741");
            });

            modelBuilder.Entity<ConferenceRoomBooking>(entity =>
            {
                entity.Property(e => e.AttendeeName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1500)
                    .IsUnicode(false);

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ConferenceRoomBooking)
                    .HasForeignKey(d => d.BookedByUid)
                    .HasConstraintName("FK__Conferenc__Booke__27B9C2CD");

                entity.HasOne(d => d.ConferenceRoom)
                    .WithMany(p => p.ConferenceRoomBooking)
                    .HasForeignKey(d => d.ConferenceRoomId)
                    .HasConstraintName("FK__Conferenc__Confe__26C59E94");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(e => e.CurrName)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.CurrSign)
                    .IsRequired()
                    .HasMaxLength(3);

                entity.Property(e => e.Modified).HasColumnType("datetime");
            });

            modelBuilder.Entity<CurrentOpening>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(100);

                entity.Property(e => e.Min_Experience)
                    .HasColumnName("Min_Experience")
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Post).HasMaxLength(150);

                entity.Property(e => e.Small_Description).HasColumnName("Small_Description");

                entity.Property(e => e.Technology).HasMaxLength(250);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.CurrentOpening)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK__CurrentOp__Depar__18AC8967");
            });

            modelBuilder.Entity<DailyThought>(entity =>
            {
                entity.Property(e => e.Thought1)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Thought2)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DeptId);

                entity.HasIndex(e => e.Deptcode)
                    .HasName("UQ__Departme__AC900526263B8EAF")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("IX_Department")
                    .IsUnique();

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Deptcode)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.IP)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<DesignerManagement>(entity =>
            {
                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AddedUid).HasColumnName("AddedUID");

                entity.Property(e => e.AssignUid).HasColumnName("AssignUID");

                entity.Property(e => e.DesignerDesription).IsUnicode(false);

                entity.Property(e => e.Priority)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TaskCompletedDate).HasColumnType("datetime");

                entity.HasOne(d => d.AddedU)
                    .WithMany(p => p.DesignerManagementAddedU)
                    .HasForeignKey(d => d.AddedUid)
                    .HasConstraintName("FK_DesignerManagement_UserLogin1");

                entity.HasOne(d => d.AssignU)
                    .WithMany(p => p.DesignerManagementAssignU)
                    .HasForeignKey(d => d.AssignUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DesignerManagement_UserLogin");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.DesignerManagement)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DesignerManagement_Project");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.Property(e => e.Condition).HasMaxLength(500);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PMUid).HasColumnName("PMUid");

                entity.Property(e => e.SimNetwork).HasMaxLength(50);

                entity.Property(e => e.SimNumber).HasMaxLength(20);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.DeviceCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Device_UserLogin_CreatedByUid");

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.DeviceModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Pmu)
                    .WithMany(p => p.DevicePmu)
                    .HasForeignKey(d => d.PMUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DeviceAccessoriesMap>(entity =>
            {
                entity.HasKey(e => new { e.DeviceDetailId, e.AccessoryId });

                entity.HasOne(d => d.Accessory)
                    .WithMany(p => p.DeviceAccessoriesMap)
                    .HasForeignKey(d => d.AccessoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DeviceAcc__Acces__40DA7652");

                entity.HasOne(d => d.DeviceDetail)
                    .WithMany(p => p.DeviceAccessoriesMap)
                    .HasForeignKey(d => d.DeviceDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DeviceAcc__Devic__3FE65219");
            });

            modelBuilder.Entity<DeviceCategory>(entity =>
            {
                entity.Property(e => e.DeviceCategoryname)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<DeviceDetail>(entity =>
            {
                entity.Property(e => e.AssignedDateTime).HasColumnType("datetime");

                entity.Property(e => e.Condition).HasMaxLength(500);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.SubmitDateTime).HasColumnType("datetime");

                entity.Property(e => e.SerialNumber).HasMaxLength(100).IsUnicode(false);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.DeviceDetailAssignedToU)
                    .HasForeignKey(d => d.AssignedToUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.DeviceDetailCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.DeviceDetails)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_DeviceDetail_Device");

                entity.HasOne(d => d.ModifyByU)
                    .WithMany(p => p.DeviceDetailModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SubmitToU)
                    .WithMany(p => p.DeviceDetailSubmitToU)
                    .HasForeignKey(d => d.SubmitToUid);
            });

            modelBuilder.Entity<DeviceDeviceHistory>(entity =>
            {
                entity.Property(e => e.AddedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.StartDateTime).HasColumnType("datetime");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.DeviceDeviceInfo)
                    .WithMany(p => p.DeviceDeviceHistory)
                    .HasForeignKey(d => d.DeviceDeviceInfoId)
                    .HasConstraintName("FK__DeviceDev__Devic__04BA9F53");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.DeviceDeviceHistory)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__DeviceDevic__Uid__05AEC38C");
            });

            modelBuilder.Entity<DeviceDeviceInfo>(entity =>
            {
                entity.Property(e => e.DeviceDeviceInfoName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.DeviceCategory)
                    .WithMany(p => p.DeviceDeviceInfo)
                    .HasForeignKey(d => d.DeviceCategoryId)
                    .HasConstraintName("FK__DeviceDev__Devic__7FF5EA36");

                entity.HasOne(d => d.Pm)
                    .WithMany(p => p.DeviceDeviceInfo)
                    .HasForeignKey(d => d.PmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DeviceDevi__PmId__097F5470");
            });

            modelBuilder.Entity<DomainType>(entity =>
            {
                entity.HasKey(e => e.DomainId);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Alias).HasMaxLength(100);

                entity.Property(e => e.DomainName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Alias).HasMaxLength(100);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EstimateHour>(entity =>
            {
                entity.Property(e => e.Bauid).HasColumnName("BAUId");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Crmid).HasColumnName("CRMId");

                entity.Property(e => e.EstimatedHour).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.RequirementName).HasMaxLength(500);

                entity.Property(e => e.Tluid).HasColumnName("TLUId");

                entity.HasOne(d => d.EstimateHourFileNameType)
                    .WithMany(p => p.EstimateHour)
                    .HasForeignKey(d => d.EstimateHourFileNameTypeId);

                entity.HasOne(d => d.Bau)
                    .WithMany(p => p.EstimateHourBau)
                    .HasForeignKey(d => d.Bauid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EstimateHoure_UserLogin_BAUId");

                entity.HasOne(d => d.Domain)
                    .WithMany(p => p.EstimateHour)
                    .HasForeignKey(d => d.DomainId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Tlu)
                    .WithMany(p => p.EstimateHourTlu)
                    .HasForeignKey(d => d.Tluid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<EstimateHourParentTechnology>(entity =>
            {
                entity.HasKey(e => new { e.EstimateHourId, e.TechnologyParentId });

                entity.HasOne(d => d.EstimateHour)
                    .WithMany(p => p.EstimateHourParentTechnology)
                    .HasForeignKey(d => d.EstimateHourId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TechnologyParent)
                    .WithMany(p => p.EstimateHourParentTechnology)
                    .HasForeignKey(d => d.TechnologyParentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<EstimateHourTechnology>(entity =>
            {
                entity.HasKey(e => new { e.EstimateHourId, e.TechnologyId });

                entity.HasOne(d => d.EstimateHour)
                    .WithMany(p => p.EstimateHourTechnology)
                    .HasForeignKey(d => d.EstimateHourId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ElanceAssignedJob>(entity =>
            {
                entity.Property(e => e.ElanceAssignedJobId)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ElanceJobId).HasColumnType("numeric(18, 0)");

                entity.HasOne(d => d.ElanceJob)
                    .WithMany(p => p.ElanceAssignedJob)
                    .HasForeignKey(d => d.ElanceJobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ElanceAssignedJob_ElanceAssignedJob");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ElanceAssignedJob)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ElanceAssignedJob_UserLogin");
            });

            modelBuilder.Entity<ElanceCredential>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccessToken)
                    .HasColumnName("Access_token")
                    .HasMaxLength(100);

                entity.Property(e => e.ElanceClientId).HasMaxLength(50);

                entity.Property(e => e.ElanceClientSecret).HasMaxLength(50);

                entity.Property(e => e.ElanceCode).HasMaxLength(100);

                entity.Property(e => e.RedirectUri)
                    .HasColumnName("RedirectURI")
                    .HasMaxLength(100);

                entity.Property(e => e.RefreshToken)
                    .HasColumnName("Refresh_token")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ElanceJobDetails>(entity =>
            {
                entity.HasKey(e => e.ElanceJobId);

                entity.Property(e => e.ElanceJobId)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Budget).HasMaxLength(50);

                entity.Property(e => e.Category)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ClientCountry)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ClientCountryCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ClientName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ClientUserId).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.IsAwarded).HasColumnName("isAwarded");

                entity.Property(e => e.IsAwardedTii).HasColumnName("IsAwardedTII");

                entity.Property(e => e.JobId).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.JobName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.JobUrl)
                    .HasColumnName("JobURL")
                    .HasMaxLength(500);

                entity.Property(e => e.PostedDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Subcategory)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<EmpLateActivity>(entity =>
            {
                entity.HasKey(e => e.ElactId);

                entity.Property(e => e.ElactId).HasColumnName("ELActId");

                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModify)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ElactDate)
                    .HasColumnName("ELActDate")
                    .HasColumnType("date");

                entity.Property(e => e.ElactTime)
                    .HasColumnName("ELActTime")
                    .HasMaxLength(10);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(20)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.EmpLateActivity)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__EmpLateActi__Uid__23893F36");
            });

            modelBuilder.Entity<EmployeeActivity>(entity =>
            {
                entity.HasKey(e => e.ActivityId);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Comments).HasColumnType("ntext");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<EmployeeAnswers>(entity =>
            {
                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Comments).HasColumnType("ntext");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.Appraisal)
                    .WithMany(p => p.EmployeeAnswers)
                    .HasForeignKey(d => d.AppraisalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeAnswers_Appraisal");

                entity.HasOne(d => d.Q)
                    .WithMany(p => p.EmployeeAnswers)
                    .HasForeignKey(d => d.Qid)
                    .HasConstraintName("FK_EmployeeAnswers_Questions");
            });

            modelBuilder.Entity<EmployeeAppraise>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ClientComment).HasColumnType("ntext");

                entity.Property(e => e.ClientDate).HasColumnType("datetime");

                entity.Property(e => e.IP)
                    .HasColumnName("IP")
                    .HasMaxLength(100);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.TlComment).HasColumnType("ntext");

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.EmployeeAppraiseEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeAppraise_UserLogin2");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.EmployeeAppraiseUser)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_EmployeeAppraise_UserLogin");
            });

            modelBuilder.Entity<EmployeeComplaint>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ClientComment).HasColumnType("ntext");

                entity.Property(e => e.ClientDate).HasColumnType("datetime");

                entity.Property(e => e.EmpComment).HasColumnType("ntext");

                entity.Property(e => e.EmpDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Priority)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TlComment).HasColumnType("ntext");

                entity.Property(e => e.Tldate)
                    .HasColumnName("TLDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeComplaintEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeComplaint_UserLogin1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.EmployeeComplaintUser)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_EmployeeComplaint_UserLogin");
            });

            modelBuilder.Entity<EmployeeFeedback>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmpPmuid).HasColumnName("EmpPMUid");

                entity.Property(e => e.LeavingDate).HasColumnType("datetime");

                entity.Property(e => e.Lfprofile).HasColumnName("LFProfile");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.ReviewLink).HasMaxLength(100);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.EmployeeFeedback)
                    .HasForeignKey(d => d.EmpUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EmployeeFee__Uid__64F7DB37");
            });

            modelBuilder.Entity<EmployeeFeedbackRank>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<EmployeeFeedbackRankStatus>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeFeedbackRankId, e.EmployeeFeedbackId, e.FeedBackStatus });

                entity.HasOne(d => d.EmployeeFeedback)
                    .WithMany(p => p.EmployeeFeedbackRankStatus)
                    .HasForeignKey(d => d.EmployeeFeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EmployeeF__Emplo__76226739");

                entity.HasOne(d => d.EmployeeFeedbackRank)
                    .WithMany(p => p.EmployeeFeedbackRankStatus)
                    .HasForeignKey(d => d.EmployeeFeedbackRankId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EmployeeF__Emplo__752E4300");
            });

            modelBuilder.Entity<EmployeeFeedbackReason>(entity =>
            {
                entity.Property(e => e.Color).HasMaxLength(15);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<EmployeeFeedbackReasonMapping>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeFeedbackId, e.EmployeeFeedbackReasonId });

                entity.HasOne(d => d.EmployeeFeedback)
                    .WithMany(p => p.EmployeeFeedbackReasonMapping)
                    .HasForeignKey(d => d.EmployeeFeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EmployeeF__Emplo__6F7569AA");

                entity.HasOne(d => d.EmployeeFeedbackReason)
                    .WithMany(p => p.EmployeeFeedbackReasonMapping)
                    .HasForeignKey(d => d.EmployeeFeedbackReasonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EmployeeF__Emplo__70698DE3");
            });

            modelBuilder.Entity<EmployeeMedicalData>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Designation).HasMaxLength(200);

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.EmployeeCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.EmployeeMedicalData)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Employeemedicaluser");


                entity.Property(e => e.PremiumPerMonth).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PremiumTotal).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<EmployeeProject>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Comments).HasColumnType("ntext");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PeriodFrom).HasColumnType("date");

                entity.Property(e => e.PeriodTo).HasColumnType("date");

                entity.Property(e => e.Tlcomments)
                    .HasColumnName("TLComments")
                    .HasColumnType("ntext");

                entity.Property(e => e.Tlstatus)
                    .HasColumnName("TLStatus")
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.InverseIdNavigation)
                    .HasForeignKey<EmployeeProject>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeProject_EmployeeProject");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.EmployeeProject)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_EmployeeProject_Project");

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.EmployeeProject)
                    .HasForeignKey(d => d.Role)
                    .HasConstraintName("FK_EmployeeProject_Role");
            });

            modelBuilder.Entity<EmployeeRelativeMedicalData>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.EmployeeMedical)
                    .WithMany(p => p.EmployeeRelativeMedicalDatas)
                    .HasForeignKey(d => d.EmployeeMedicalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_employeereative");
            });

            modelBuilder.Entity<Escalation>(entity =>
            {
                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateofEscalation).HasColumnType("date");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.EscalationTypeNavigation)
                    .WithMany(p => p.Escalation)
                    .HasForeignKey(d => d.EscalationType)
                    .HasConstraintName("FK_EscalationType");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Escalation)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Escalatio__Proje__16BA1F54");

                entity.HasOne(d => d.RootCauseNavigation)
                    .WithMany(p => p.Escalation)
                    .HasForeignKey(d => d.RootCause)
                    .HasConstraintName("FK__Escalatio__RootC__17AE438D");

                entity.HasOne(d => d.AddedByU)
                    .WithMany(p => p.Escalation)
                    .HasForeignKey(d => d.AddedByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<EscalationConclusion>(entity =>
            {
                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.AddedByU)
                    .WithMany(p => p.EscalationConclusion)
                    .HasForeignKey(d => d.AddedByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Escalatio__Added__2AC11801");

                entity.HasOne(d => d.Escalation)
                    .WithMany(p => p.EscalationConclusion)
                    .HasForeignKey(d => d.EscalationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<EscalationDocument>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentPath).HasMaxLength(500);

                entity.HasOne(d => d.Escalation)
                    .WithMany(p => p.EscalationDocument)
                    .HasForeignKey(d => d.EscalationId)
                    .HasConstraintName("FK__Escalatio__Escal__316E1590");
            });

            modelBuilder.Entity<EscalationRootCause>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<EscalationType>(entity =>
            {
                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<EsculationForUser>(entity =>
            {
                entity.HasKey(e => new { e.EsculationId, e.Uid })
                    .HasName("PK__Esculati__798ADCC71D671CE3");

                entity.HasOne(d => d.Esculation)
                    .WithMany(p => p.EsculationForUser)
                    .HasForeignKey(d => d.EsculationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Esculatio__Escul__1F4F6555");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.EsculationForUser)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EsculationF__Uid__2043898E");
            });

            modelBuilder.Entity<EsculationFoundForUser>(entity =>
            {
                entity.HasKey(e => new { e.EsculationId, e.Uid })
                    .HasName("PK__Esculati__798ADCC7231FF639");

                entity.HasOne(d => d.Esculation)
                    .WithMany(p => p.EsculationFoundForUser)
                    .HasForeignKey(d => d.EsculationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Esculatio__Escul__25083EAB");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.EsculationFoundForUser)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__EsculationF__Uid__25FC62E4");
            });

            modelBuilder.Entity<EstimateCountry>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.EstimateCountry)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("EstimateCountry_Currency_CurrencyId");
            });

            modelBuilder.Entity<EstimateDocument>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DocumentPath).HasMaxLength(400);

                entity.Property(e => e.Industry).HasMaxLength(400);

                entity.Property(e => e.MockupDocument).HasMaxLength(500);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.OtherDocument).HasMaxLength(500);
                entity.Property(e => e.Flowcharts).HasMaxLength(500);

                entity.Property(e => e.Tags).HasMaxLength(400);

                entity.Property(e => e.Technology)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Uid_UploadedBy).HasColumnName("Uid_UploadedBy");

                entity.Property(e => e.Wireframe_MockupsDoc)
                    .HasColumnName("Wireframe_MockupsDoc")
                    .HasMaxLength(300);

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.EstimateDocuments)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("fk_lead");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.EstimateDocument)
                    .HasForeignKey(d => d.Uid_UploadedBy)
                    .HasConstraintName("FK_UploadedBy");
            });

            modelBuilder.Entity<EstimateHostingPackage>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.HostingCost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.PackageDetail).IsRequired();

                entity.Property(e => e.SetupCost).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.EstimateHostingPackage)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("EstimateRole_EstimateHostingPackage_CountryId");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.EstimateHostingPackage)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("EstimateHostingPackage_Currency_CurrencyId");
            });

            modelBuilder.Entity<EstimateHostingPackageTechnology>(entity =>
            {
                entity.HasKey(e => new { e.EstimateHostingPackageId, e.EstimateTechnologyId })
                    .HasName("PK__Estimate__D6A24DCE0A895299");

                entity.HasOne(d => d.EstimateHostingPackage)
                    .WithMany(p => p.EstimateHostingPackageTechnology)
                    .HasForeignKey(d => d.EstimateHostingPackageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EstimateHostingPackage_EstimateHostingPackageTechnology_EstimateHostingPackageId");

                entity.HasOne(d => d.EstimateTechnology)
                    .WithMany(p => p.EstimateHostingPackageTechnology)
                    .HasForeignKey(d => d.EstimateTechnologyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EstimateTechnology_EstimateHostingPackageTechnology_EstimateTechnologyId");
            });

            modelBuilder.Entity<Examination>(entity =>
            {
                entity.HasKey(e => e.ExamId);

                entity.Property(e => e.ExamId).HasColumnName("ExamID");

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExamCode)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MaxTime)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<ExamQuestion>(entity =>
            {
                entity.HasKey(e => e.QuestionId);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.TechnologyId).HasColumnName("TechnologyID");

                entity.HasOne(d => d.QuestionLevelNavigation)
                    .WithMany(p => p.ExamQuestionQuestionLevelNavigation)
                    .HasForeignKey(d => d.QuestionLevel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExamQuest__Quest__0E240DFC");

                entity.HasOne(d => d.QuestionTypeNavigation)
                    .WithMany(p => p.ExamQuestionQuestionTypeNavigation)
                    .HasForeignKey(d => d.QuestionType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExamQuest__Quest__0D2FE9C3");

                entity.HasOne(d => d.Technology)
                    .WithMany(p => p.ExamQuestion)
                    .HasForeignKey(d => d.TechnologyId)
                    .HasConstraintName("FK__ExamQuest__Techn__0C3BC58A");
            });

            modelBuilder.Entity<ExamQuestionAnswerDetail>(entity =>
            {
                entity.HasKey(e => e.Qaid);

                entity.Property(e => e.Qaid).HasColumnName("QAId");

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Answer)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.Property(e => e.IsCorrect).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.QuestId).HasColumnName("QuestID");

                entity.HasOne(d => d.Quest)
                    .WithMany(p => p.ExamQuestionAnswerDetail)
                    .HasForeignKey(d => d.QuestId)
                    .HasConstraintName("FK__ExamQuest__Quest__16B953FD");
            });

            modelBuilder.Entity<ExamQuestionDetail>(entity =>
            {
                entity.HasKey(e => e.Eqid);

                entity.HasIndex(e => new { e.ExamId, e.QuestionId })
                    .HasName("uc_ExamQuestionDetail")
                    .IsUnique();

                entity.Property(e => e.Eqid).HasColumnName("EQID");

                entity.Property(e => e.ExamId).HasColumnName("ExamID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamQuestionDetail)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExamQuest__ExamI__27E3DFFF");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ExamQuestionDetail)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExamQuest__Quest__28D80438");
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ExpenseDate).HasColumnType("date");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.ReimburseDate).HasColumnType("datetime");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ExpenseApprovedByU)
                    .HasForeignKey(d => d.ApprovedByUid);

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.ExpenseCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Expense_Currency");

                entity.HasOne(d => d.UserLogin2)
                    .WithMany(p => p.ExpenseModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserLogin3)
                    .WithMany(p => p.ExpenseReimbursedByU)
                    .HasForeignKey(d => d.ReimbursedByUid);
            });

            modelBuilder.Entity<FinancialYear>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(20);
            });

            modelBuilder.Entity<Forecasting>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.AddedPersonUId).HasColumnName("AddedPersonUId");

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Groups).HasMaxLength(250);

                entity.Property(e => e.ProjectDescription)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.TentiveDate).HasColumnType("datetime");

                entity.Property(e => e.AddedPersonUId).HasColumnName("AddedPersonUId");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.Forecasting)
                    .HasForeignKey(d => d.AddedPersonUId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Forecasting_UserLogin");


                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Forecasting)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_Forecasting_Client");

                entity.HasOne(d => d.ProjectLead)
                    .WithMany(p => p.Forecastings)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("FK_Forecasting_ProjectLead");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Forecasting)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Forecasting_Project");

                entity.HasOne(d => d.ReviewedU)
                   .WithMany(p => p.ForecastingReviewedU)
                   .HasForeignKey(d => d.ReviewedUid);
            });

            modelBuilder.Entity<ProjectOtherPm>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.Pmuid });

                entity.ToTable("ProjectOtherPM");

                entity.Property(e => e.Pmuid).HasColumnName("PMUId");

                entity.HasOne(d => d.Pmu)
                    .WithMany(p => p.ProjectOtherPm)
                    .HasForeignKey(d => d.Pmuid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectOt__PMUId__78E9C54B");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectOtherPm)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectOt__Proje__77F5A112");
            });

            modelBuilder.Entity<ForecastingDepartment>(entity =>
            {
                entity.HasKey(e => new { e.ForecastingId, e.DepartmentId });

                entity.ToTable("Forecasting_Department");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.ForecastingDepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Forecasti__Depar__14A6EE59");

                entity.HasOne(d => d.Forecasting)
                    .WithMany(p => p.ForecastingDepartment)
                    .HasForeignKey(d => d.ForecastingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Forecasti__Forec__13B2CA20");
            });

            modelBuilder.Entity<ForumFeedback>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.Forum)
                    .WithMany(p => p.ForumFeedback)
                    .HasForeignKey(d => d.ForumId)
                    .HasConstraintName("FK_ForumFeedback_ForumFeedback");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ForumFeedback)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ForumFeedback_UserLogin");
            });

            modelBuilder.Entity<Forums>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("ntext");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasMaxLength(500);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Forums)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Forums_UserLogin");
            });

            modelBuilder.Entity<FrontMenu>(entity =>
            {
                entity.HasKey(e => e.MenuId);

                entity.Property(e => e.ChildPages).HasMaxLength(1000);

                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModify)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.MenuDisplayName).HasMaxLength(200);

                entity.Property(e => e.MenuName).HasMaxLength(200);

                entity.Property(e => e.PageName).HasMaxLength(200);
            });

            modelBuilder.Entity<FullLeave>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.HasOne(d => d.Appraisal)
                    .WithMany(p => p.FullLeave)
                    .HasForeignKey(d => d.AppraisalId)
                    .HasConstraintName("FK_FullLeave_Appraisal");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FullLeave)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_FullLeave_UserLogin");
            });

            modelBuilder.Entity<HalfLeave>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.HasOne(d => d.Appraisal)
                    .WithMany(p => p.HalfLeave)
                    .HasForeignKey(d => d.AppraisalId)
                    .HasConstraintName("FK_HalfLeave_Appraisal");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.HalfLeave)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_HalfLeave_UserLogin");
            });

            modelBuilder.Entity<IntwExperience>(entity =>
            {
                entity.Property(e => e.Experience).HasMaxLength(50);
            });

            modelBuilder.Entity<IntwQues>(entity =>
            {
                entity.Property(e => e.IntwQuesId)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Modifydate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.HasOne(d => d.IntwQuestype)
                    .WithMany(p => p.IntwQues)
                    .HasForeignKey(d => d.IntwQuestypeId)
                    .HasConstraintName("FK_IntwQues_IntwQuestypeId");

                entity.HasOne(d => d.IntwTechnology)
                    .WithMany(p => p.IntwQues)
                    .HasForeignKey(d => d.IntwTechnologyId)
                    .HasConstraintName("FK_IntwQues_IntwTechnologyId");
            });

            modelBuilder.Entity<IntwQuesExp>(entity =>
            {
                entity.Property(e => e.IntwQuesId).HasColumnType("numeric(18, 0)");

                entity.HasOne(d => d.IntwExperience)
                    .WithMany(p => p.IntwQuesExp)
                    .HasForeignKey(d => d.IntwExperienceId)
                    .HasConstraintName("FK_IntwQuesExp_ExperienceID");

                entity.HasOne(d => d.IntwQues)
                    .WithMany(p => p.IntwQuesExp)
                    .HasForeignKey(d => d.IntwQuesId)
                    .HasConstraintName("FK_IntwQuesExp_IntwQuesId");
            });

            modelBuilder.Entity<IntwQuestype>(entity =>
            {
                entity.Property(e => e.Marks).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.TypeName).HasMaxLength(50);
            });

            modelBuilder.Entity<IntwQusOptions>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.IntwQuesId).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.OptionTitle).HasMaxLength(500);

                entity.HasOne(d => d.IntwQues)
                    .WithMany(p => p.IntwQusOptions)
                    .HasForeignKey(d => d.IntwQuesId)
                    .HasConstraintName("FK_IntwQusOptions_IntwQuesId");
            });

            modelBuilder.Entity<IntwTechnology>(entity =>
            {
                entity.Property(e => e.TechnologyName).HasMaxLength(50);
            });

            modelBuilder.Entity<IntwUser>(entity =>
            {
                entity.HasIndex(e => e.EmailId)
                    .HasName("UQ__IntwUser__7ED91AEE2D67AF2B")
                    .IsUnique();

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.EmailId)
                    .HasColumnName("EmailID")
                    .HasMaxLength(100);

                entity.Property(e => e.Mobile).HasMaxLength(50);

                entity.Property(e => e.Modifydate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.UserResume)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.IntwTechnology)
                    .WithMany(p => p.IntwUser)
                    .HasForeignKey(d => d.IntwTechnologyId)
                    .HasConstraintName("FK_IntwUser_IntwTechnologyId");
            });

            modelBuilder.Entity<IntwUserAnswer>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.IntwUserQuesId).HasColumnType("numeric(18, 0)");

                entity.HasOne(d => d.IntwQusOptions)
                    .WithMany(p => p.IntwUserAnswer)
                    .HasForeignKey(d => d.IntwQusOptionsId)
                    .HasConstraintName("FK_IntwUserAnswer_IntwQusOptions");

                entity.HasOne(d => d.IntwUser)
                    .WithMany(p => p.IntwUserAnswer)
                    .HasForeignKey(d => d.IntwUserId)
                    .HasConstraintName("FK_IntwUserAnswer_IntwUser");

                entity.HasOne(d => d.IntwUserQues)
                    .WithMany(p => p.IntwUserAnswer)
                    .HasForeignKey(d => d.IntwUserQuesId)
                    .HasConstraintName("FK_IntwUserAnswer_IntwUserQues");

                entity.HasOne(d => d.IntwUserSession)
                    .WithMany(p => p.IntwUserAnswer)
                    .HasForeignKey(d => d.IntwUserSessionId)
                    .HasConstraintName("FK_IntwUserAnswer_IntwUserSession");
            });

            modelBuilder.Entity<IntwUserQues>(entity =>
            {
                entity.Property(e => e.IntwUserQuesId)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Adddate).HasColumnType("datetime");

                entity.Property(e => e.IntwQuesid).HasColumnType("numeric(18, 0)");

                entity.HasOne(d => d.IntwQues)
                    .WithMany(p => p.IntwUserQues)
                    .HasForeignKey(d => d.IntwQuesid)
                    .HasConstraintName("FK_IntwUserQues_IntwQues");

                entity.HasOne(d => d.IntwUserSession)
                    .WithMany(p => p.IntwUserQues)
                    .HasForeignKey(d => d.IntwUserSessionid)
                    .HasConstraintName("FK_IntwUserQues_IntwUserSession");

                entity.HasOne(d => d.IntwUser)
                    .WithMany(p => p.IntwUserQues)
                    .HasForeignKey(d => d.IntwUserid)
                    .HasConstraintName("FK_IntwUserQues_IntwUser");
            });

            modelBuilder.Entity<IntwUserSession>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.IntwQuesExp)
                    .WithMany(p => p.IntwUserSession)
                    .HasForeignKey(d => d.IntwQuesExpId)
                    .HasConstraintName("FK_IntwUserSession_IntwQuesExpId");

                entity.HasOne(d => d.IntwUser)
                    .WithMany(p => p.IntwUserSession)
                    .HasForeignKey(d => d.IntwUserId)
                    .HasConstraintName("FK_IntwUserSession_IntwUserId");
            });

            modelBuilder.Entity<Investment>(entity =>
            {
                entity.Property(e => e.AttendanceCode).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DOB)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FatherName).HasMaxLength(200);

                entity.Property(e => e.HomeAddress).IsRequired();

                entity.Property(e => e.IsDraft)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PAN)
                    .IsRequired()
                    .HasColumnName("PAN")
                    .HasMaxLength(20);

                entity.HasOne(d => d.FinancialYear)
                    .WithMany(p => p.Investment)
                    .HasForeignKey(d => d.FinancialYearId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Investmen__Finan__0EB90AD9");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.Investment)
                    .HasForeignKey(d => d.UserloginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Investmen__Userl__0DC4E6A0");
            });

            modelBuilder.Entity<InvestmentDocument>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentName).HasMaxLength(250);

                entity.Property(e => e.DocumentPath).IsRequired();

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.InvestmentNavigation)
                    .WithMany(p => p.InvestmentDocuments)
                    .HasForeignKey(d => d.Investment)
                    .HasConstraintName("FK__Investmen__Inves__2878DCDC");

                entity.HasOne(d => d.InvestmentType)
                    .WithMany(p => p.InvestmentDocument)
                    .HasForeignKey(d => d.InvestmentTypeId)
                    .HasConstraintName("FK__Investmen__Inves__296D0115");
            });

            modelBuilder.Entity<InvestmentMonth>(entity =>
            {
                entity.Property(e => e.ModifyDate).HasColumnType("datetime");
                entity.Property(e => e.MonthlyRent).HasColumnType("decimal(18, 2)");
                entity.HasOne(d => d.Investment)
                    .WithMany(p => p.InvestmentMonths)
                    .HasForeignKey(d => d.InvestmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Investmen__Inves__137DBFF6");
            });

            modelBuilder.Entity<InvestmentType>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.FinancialYear)
                    .WithMany(p => p.InvestmentType)
                    .HasForeignKey(d => d.FinancialYearId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Investmen__Finan__09003183");
            });

            modelBuilder.Entity<InvestmentTypeAmountMap>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.Investment)
                    .WithMany(p => p.InvestmentTypeAmountMaps)
                    .HasForeignKey(d => d.InvestmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Investmen__Inves__1936994C");

                entity.HasOne(d => d.InvestmentType)
                    .WithMany(p => p.InvestmentTypeAmountMap)
                    .HasForeignKey(d => d.InvestmentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Investmen__Inves__18427513");
            });

            modelBuilder.Entity<JobReference>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Attacchment).HasMaxLength(250);

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.IP)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.MobileNo).HasMaxLength(20);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.ReferBy_UserLoginId).HasColumnName("ReferBy_UserLoginId");

                entity.Property(e => e.Small_Desc).HasColumnName("Small_Desc");

                entity.HasOne(d => d.CurrentOpening)
                    .WithMany(p => p.JobReferences)
                    .HasForeignKey(d => d.CurrentOpeningId)
                    .HasConstraintName("FK__JobRefere__Curre__28E2F130");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.JobReference)
                    .HasForeignKey(d => d.ReferBy_UserLoginId)
                    .HasConstraintName("FK__JobRefere__Refer__29D71569");
            });

            modelBuilder.Entity<KnowledgeBase>(entity =>
            {
                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.FilePath).HasMaxLength(500);

                entity.Property(e => e.Instructions).HasColumnType("text");

                entity.Property(e => e.KnowledgeType).HasDefaultValueSql("((1))");

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.KnowledgeBase)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CodeLibrary_UserLogin");
            });

            modelBuilder.Entity<KnowledgeDepartment>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DeptId).HasColumnName("DeptID");

                entity.HasOne(d => d.Code)
                    .WithMany(p => p.KnowledgeDepartment)
                    .HasForeignKey(d => d.CodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Knowledge__CodeI__232A17DA");

                entity.HasOne(d => d.Dept)
                    .WithMany(p => p.KnowledgeDepartment)
                    .HasForeignKey(d => d.DeptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Knowledge__DeptI__241E3C13");
            });

            modelBuilder.Entity<KnowledgeTech>(entity =>
            {
                entity.HasOne(d => d.Code)
                    .WithMany(p => p.KnowledgeTech)
                    .HasForeignKey(d => d.CodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CodeTech_CodeLibrary");

                entity.HasOne(d => d.Tech)
                    .WithMany(p => p.KnowledgeTech)
                    .HasForeignKey(d => d.TechId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CodeTech_Technology");
            });

            modelBuilder.Entity<LateHour>(entity =>
            {
                entity.Property(e => e.DayOfDate).HasColumnType("date");

                entity.Property(e => e.EarlyReason).HasMaxLength(2000);

                entity.Property(e => e.LateReason).HasMaxLength(2000);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.WorkAtHome).HasMaxLength(2000);

                entity.Property(e => e.WorkFromHome).HasMaxLength(4000);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.LateHourModifiedByU)
                    .HasForeignKey(d => d.ModifiedByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LateHour__Modifi__0EEE1503");

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.LateHourU)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LateHour__Uid__0DF9F0CA");
            });

            modelBuilder.Entity<LeadClient>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("UQ_Email")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.PMUid).HasColumnName("PMUid");
            });


            modelBuilder.Entity<Leadership>(entity =>
            {
                entity.HasKey(e => e.Lid);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<LeadStatu>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.HasIndex(e => e.StatusName)
                    .HasName("UQ__LeadStat__05E7698A52442E1F")
                    .IsUnique();

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.BCC)
                    .HasColumnName("BCC")
                    .HasMaxLength(500);

                entity.Property(e => e.CC)
                    .HasColumnName("CC")
                    .HasMaxLength(500);

                entity.Property(e => e.FromEmail).HasMaxLength(100);

                entity.Property(e => e.IP)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.Property(e => e.MailContent).HasColumnType("ntext");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.To).HasMaxLength(500);
            });

            modelBuilder.Entity<LeadTechnician>(entity =>
            {
                entity.HasKey(e => e.AutoId);

                entity.HasIndex(e => new { e.LeadId, e.TechnicianId })
                    .HasName("UQ_LeadId_TechnicianId")
                    .IsUnique();

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.LeadTechnicians)
                    .HasForeignKey(d => d.LeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTechn__LeadI__795DFB40");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.LeadTechnician)
                    .HasForeignKey(d => d.TechnicianId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTechn__Techn__7A521F79");
            });

            modelBuilder.Entity<LeadTechnicianArchive>(entity =>
            {
                entity.HasKey(e => e.AutoId);

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.LeadTechnicianArchive)
                    .HasForeignKey(d => d.LeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTechn__LeadI__60D24498");

                entity.HasOne(d => d.Technician)
                    .WithMany(p => p.LeadTechnicianArchive)
                    .HasForeignKey(d => d.TechnicianId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTechn__Techn__61C668D1");
            });

            modelBuilder.Entity<LeadTransaction>(entity =>
            {
                entity.HasKey(e => e.TransId);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ConversionDate).HasColumnType("datetime");

                entity.Property(e => e.Doc).HasMaxLength(300);

                entity.Property(e => e.Notes).HasColumnType("ntext");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.LeadTransaction)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTrans__Added__10416098");

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.LeadTransactions)
                    .HasForeignKey(d => d.LeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTrans__LeadI__0E591826");

                entity.HasOne(d => d.LeadStatu)
                    .WithMany(p => p.LeadTransaction)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTrans__Statu__0F4D3C5F");

                entity.HasOne(d => d.ProjectClosure)
                .WithMany(p => p.LeadTransaction)
                .HasForeignKey(d => d.ProjectClosureId);

            });

            modelBuilder.Entity<LeadTransactionArchive>(entity =>
            {
                entity.HasKey(e => e.TransId);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Doc).HasMaxLength(300);

                entity.Property(e => e.Notes).HasColumnType("ntext");

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.LeadTransactionArchive)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTrans__Added__5B196B42");

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.LeadTransactionArchive)
                    .HasForeignKey(d => d.LeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTrans__LeadI__593122D0");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.LeadTransactionArchive)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LeadTrans__Statu__5A254709");
            });

            modelBuilder.Entity<LeaveActivity>(entity =>
            {
                entity.HasKey(e => e.LeaveId);

                entity.Property(e => e.AdjustId).HasColumnName("AdjustID");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.DateModify).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.Reason).HasColumnType("ntext");

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.WorkAlterId).HasColumnName("WorkAlterID");

                entity.Property(e => e.WorkAlternator).HasColumnType("ntext");

                entity.HasOne(d => d.ModifyByNavigation)
                    .WithMany(p => p.LeaveActivityModifyByNavigation)
                    .HasForeignKey(d => d.ModifyBy)
                    .HasConstraintName("FK__LeaveActi__Modif__33765993");
                entity.HasOne(d => d.Adjust)
                    .WithMany(p => p.LeaveActivity)
                    .HasForeignKey(d => d.AdjustId)
                    .HasConstraintName("FK_LeaveActivity_LeaveAdjust");

                entity.HasOne(d => d.TypeMaster)
                    .WithMany(p => p.LeaveActivityLeaveTypeNavigation)
                    .HasForeignKey(d => d.LeaveType)
                    .HasConstraintName("FK__LeaveActi__Leave__48BAC3E5");

                entity.HasOne(d => d.TypeMaster1)
                    .WithMany(p => p.LeaveActivityStatusNavigation)
                    .HasForeignKey(d => d.Status)
                    .HasConstraintName("FK_LeaveActivity_TypeMaster");

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.LeaveActivities1)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveActivity_UserLogin");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.LeaveActivities)
                    .HasForeignKey(d => d.WorkAlterId)
                    .HasConstraintName("FK__LeaveActi__WorkA__68687968");

                entity.HasOne(d => d.LeaveCategoryNavigation)
                   .WithMany(p => p.LeaveActivity)
                   .HasForeignKey(d => d.LeaveCategory)
                   .HasConstraintName("FK__LeaveActi__Leave__47284C85");
            });

            modelBuilder.Entity<LeaveTypesMaster>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<LeaveActivityAdjust>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.HasOne(d => d.Adjust)
                    .WithMany(p => p.LeaveActivityAdjust)
                    .HasForeignKey(d => d.Adjustid)
                    .HasConstraintName("FK_LeaveActivityAdjust_LeaveAdjust");

                entity.HasOne(d => d.Leave)
                    .WithMany(p => p.LeaveActivityAdjust)
                    .HasForeignKey(d => d.LeaveId)
                    .HasConstraintName("FK_LeaveActivityAdjust_LeaveActivity");
            });

            modelBuilder.Entity<LeaveAdjust>(entity =>
            {
                entity.HasKey(e => e.AdjustId);

                entity.Property(e => e.ClhalfAdjustId).HasColumnName("CLHalfAdjustId");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.DateModify).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.IsCancel).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsCl).HasColumnName("IsCL");

                entity.Property(e => e.Isadjust).HasColumnName("isadjust");

                entity.Property(e => e.Reason).HasColumnType("ntext");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.LeaveAdjust)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveAdjust_UserLogin");
            });

            modelBuilder.Entity<Management>(entity =>
            {
                entity.HasKey(e => e.Mid);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<MeetingMaster>(entity =>
            {
                entity.Property(e => e.CreatedByUID).HasColumnName("CreatedByUID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByU)
                    .WithMany(p => p.MeetingMaster)
                    .HasForeignKey(d => d.CreatedByUID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MeetingMa__Creat__3BE0B70B");
            });

            modelBuilder.Entity<MeetingMinutes>(entity =>
            {
                entity.Property(e => e.ActionPoint).HasColumnType("ntext");

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Discussed).HasColumnType("ntext");

                entity.Property(e => e.MeetingDate).HasColumnType("datetime");

                entity.Property(e => e.MeetingSubject).HasMaxLength(1000);

                entity.Property(e => e.PmandTl)
                    .HasColumnName("PMandTL")
                    .HasMaxLength(700);
            });

            modelBuilder.Entity<MenuAccess>(entity =>
            {
                entity.HasKey(e => e.AccessId);

                entity.HasOne(d => d.FrontMenu)
                    .WithMany(p => p.MenuAccesses)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MenuAccess_FrontMenu");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.MenuAccessNavigation)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MenuAccess_Role");
            });



            modelBuilder.Entity<MomMeeting>(entity =>
            {
                entity.Property(e => e.AuthorByUID).HasColumnName("AuthorByUID");

                entity.Property(e => e.ChairedByUID).HasColumnName("ChairedByUID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateOfMeeting).HasColumnType("date");

                entity.Property(e => e.MeetingMasterID).HasColumnName("MeetingMasterID");

                entity.Property(e => e.MeetingTitle)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.VenueName).HasMaxLength(100);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.MomMeetingAuthorByU)
                    .HasForeignKey(d => d.AuthorByUID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__Autho__428DB49A");

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.MomMeetingChairedByU)
                    .HasForeignKey(d => d.ChairedByUID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__Chair__41999061");

                entity.HasOne(d => d.MeetingMaster)
                    .WithMany(p => p.MomMeetings)
                    .HasForeignKey(d => d.MeetingMasterID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__Meeti__40A56C28");
            });

            modelBuilder.Entity<MomMeetingDepartment>(entity =>
            {
                entity.HasKey(e => new { e.MomMeetingId, e.DepartmentId });

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.MomMeetingDepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__Depar__62065FF3");

                entity.HasOne(d => d.MomMeeting)
                    .WithMany(p => p.MomMeetingDepartment)
                    .HasForeignKey(d => d.MomMeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__MomMe__61123BBA");
            });

            modelBuilder.Entity<MomMeetingParticipant>(entity =>
            {
                entity.HasKey(e => new { e.MomMeetingId, e.Uid });

                entity.HasOne(d => d.MomMeeting)
                    .WithMany(p => p.MomMeetingParticipant)
                    .HasForeignKey(d => d.MomMeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__MomMe__5B596264");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.MomMeetingParticipant)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetingP__Uid__5C4D869D");
            });

            modelBuilder.Entity<MomMeetingTask>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Remark).HasMaxLength(500);

                entity.Property(e => e.TargetDate).HasColumnType("date");

                entity.Property(e => e.Task).HasMaxLength(500);

                entity.HasOne(d => d.MomMeeting)
                    .WithMany(p => p.MomMeetingTasks)
                    .HasForeignKey(d => d.MomMeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__MomMe__4D0B430D");
            });

            modelBuilder.Entity<MomMeetingTaskParticipant>(entity =>
            {
                entity.HasKey(e => new { e.MomMeetingTaskId, e.Uid });

                entity.HasOne(d => d.MomMeetingTask)
                    .WithMany(p => p.MomMeetingTaskParticipant)
                    .HasForeignKey(d => d.MomMeetingTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__MomMe__66CB1510");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.MomMeetingTaskParticipant)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetingT__Uid__67BF3949");
            });

            modelBuilder.Entity<MomMeetingTaskTimeLine>(entity =>
            {
                entity.Property(e => e.CommentByUid).HasColumnName("CommentByUId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.TargetDate).HasColumnType("date");
                entity.HasOne(d => d.CommentByU)
                    .WithMany(p => p.MomMeetingTaskTimeLine)
                    .HasForeignKey(d => d.CommentByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__Comme__5694AD47");

                entity.HasOne(d => d.MomMeeting)
                    .WithMany(p => p.MomMeetingTaskTimeLines)
                    .HasForeignKey(d => d.MomMeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__MomMe__55A0890E");

                entity.HasOne(d => d.MomMeetingTask)
                    .WithMany(p => p.MomMeetingTaskTimeLine)
                    .HasForeignKey(d => d.MomMeetingTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MomMeetin__MomMe__54AC64D5");

                entity.Property(e => e.TaskCommentId)
                    .HasColumnName("TaskCommentID")
                    .HasColumnType("numeric(18, 0)");

                entity.HasOne(d => d.TaskComment)
                    .WithMany(p => p.MomMeetingTaskTimeLine)
                    .HasForeignKey(d => d.TaskCommentId)
                    .HasConstraintName("FK__MomMeetin__TaskC__05F9A7A6");

            });

            modelBuilder.Entity<OfficialLeave>(entity =>
            {
                entity.HasKey(e => e.LeaveId);

                entity.Property(e => e.LeaveDate).HasColumnType("datetime");

                entity.Property(e => e.LeaveType).HasMaxLength(500);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<OrgDocument>(entity =>
            {
                entity.Property(e => e.ApprovedDate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentPath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Ver)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.OrgDocumentCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ModifyByU)
                    .WithMany(p => p.OrgDocumentModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OrgDocumentMaster)
                    .WithMany(p => p.OrgDocuments)
                    .HasForeignKey(d => d.OrgDocumentMasterId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OrgDocumentApprove>(entity =>
            {
                entity.HasKey(e => new { e.OrgDocumentId, e.ApproverUid });

                entity.Property(e => e.ApprovedDate).HasColumnType("datetime");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.OrgDocumentApprove)
                    .HasForeignKey(d => d.ApproverUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrgDocumentReview_UserLogin_ReviewerUid");

                entity.HasOne(d => d.OrgDocument)
                    .WithMany(p => p.OrgDocumentApproves)
                    .HasForeignKey(d => d.OrgDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrgDocumentReview_OrgDocument_OrgDocumentId");
            });

            modelBuilder.Entity<OrgDocumentDepartment>(entity =>
            {
                entity.HasKey(e => new { e.OrgDocumentId, e.DepartmentId });

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.OrgDocumentDepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrgDocument_Department_DepartmentId");

                entity.HasOne(d => d.OrgDocument)
                    .WithMany(p => p.OrgDocumentDepartment)
                    .HasForeignKey(d => d.OrgDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrgDocument_Department_OrgDocumentId");
            });

            modelBuilder.Entity<OrgDocumentMaster>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<OrgDocumentRole>(entity =>
            {
                entity.HasKey(e => new { e.OrgDocumentId, e.RoleId });

                entity.HasOne(d => d.OrgDocument)
                    .WithMany(p => p.OrgDocumentRole)
                    .HasForeignKey(d => d.OrgDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrgDocument_Role_OrgDocumentId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.OrgDocumentRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrgDocument_Role_RoleId");
            });

            modelBuilder.Entity<PersonalDevelopment>(entity =>
            {
                entity.HasKey(e => e.PdId);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<PfreviewQuarter>(entity =>
            {
                entity.ToTable("PFReviewQuarter");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.QuarterName).HasMaxLength(50);
            });

            modelBuilder.Entity<PfreviewQuestion>(entity =>
            {
                entity.ToTable("PFReviewQuestion");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.ReviewCategory).HasMaxLength(250);
            });

            modelBuilder.Entity<PfreviewResult>(entity =>
            {
                entity.ToTable("PFReviewResult");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.PfreviewAnswer).HasColumnName("PFReviewAnswer");

                entity.Property(e => e.PfreviewQuestionId).HasColumnName("PFReviewQuestionId");

                entity.Property(e => e.PfreviewSubmittedId).HasColumnName("PFReviewSubmittedId");

                entity.HasOne(d => d.PfreviewQuestion)
                    .WithMany(p => p.PfreviewResult)
                    .HasForeignKey(d => d.PfreviewQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PFReviewR__PFRev__17ED6F58");

                entity.HasOne(d => d.PfreviewSubmitted)
                    .WithMany(p => p.PfreviewResult)
                    .HasForeignKey(d => d.PfreviewSubmittedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PFReviewR__PFRev__16F94B1F");
            });

            modelBuilder.Entity<PfreviewSubmitted>(entity =>
            {
                entity.ToTable("PFReviewSubmitted");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.ReviewByUid).HasColumnName("ReviewBy_Uid");

                entity.Property(e => e.ReviewOnUid).HasColumnName("ReviewOn_Uid");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.ReviewByU)
                    .WithMany(p => p.PfreviewSubmittedReviewByU)
                    .HasForeignKey(d => d.ReviewByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PFReviewS__Revie__114071C9");

                entity.HasOne(d => d.ReviewOnU)
                    .WithMany(p => p.PfreviewSubmittedReviewOnU)
                    .HasForeignKey(d => d.ReviewOnUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PFReviewS__Revie__104C4D90");

                entity.HasOne(d => d.ReviewQuarterNavigation)
                    .WithMany(p => p.PfreviewSubmitted)
                    .HasForeignKey(d => d.ReviewQuarter)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PFReviewS__Revie__12349602");
            });

            modelBuilder.Entity<PILog>(entity =>
            {
                entity.ToTable("PILog");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EstimatedSchedule).HasColumnType("date");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.PotentialArea).IsRequired();

                entity.Property(e => e.ProcessName).HasMaxLength(512);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.PilogCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ModifyByU)
                    .WithMany(p => p.PilogModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Portfolio>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ClientName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CrmprojectId).HasColumnName("CRMProjectId");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(100);

                entity.Property(e => e.IsNda).HasColumnName("IsNDA");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasColumnType("ntext");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.WebsiteName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.WebsiteUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.Portfolio)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Portfolio_User_Project");
            });

            modelBuilder.Entity<PortfolioDomain>(entity =>
            {
                entity.ToTable("Portfolio_Domain");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PortfolioId).HasColumnName("PortfolioID");

                entity.HasOne(d => d.Domain)
                    .WithMany(p => p.PortfolioDomain)
                    .HasForeignKey(d => d.DomainId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Portfolio_Domain_Technology");

                entity.HasOne(d => d.Portfolio)
                    .WithMany(p => p.PortfolioDomain)
                    .HasForeignKey(d => d.PortfolioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Portfolio_Domain_Project");
            });

            modelBuilder.Entity<PortfolioTech>(entity =>
            {
                entity.ToTable("Portfolio_Tech");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PortfolioId).HasColumnName("PortfolioID");

                entity.HasOne(d => d.Portfolio)
                    .WithMany(p => p.PortfolioTech)
                    .HasForeignKey(d => d.PortfolioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Portfolio_Tech_Project");

                entity.HasOne(d => d.Tech)
                    .WithMany(p => p.PortfolioTech)
                    .HasForeignKey(d => d.TechId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Portfolio_Tech_Technology");
            });

            modelBuilder.Entity<Preference>(entity =>
            {
                entity.HasKey(e => e.PreferenceId);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AdditionalSupportEmail).HasMaxLength(3000);

                entity.Property(e => e.ELActTimeLimit).HasColumnName("ELActTimeLimit");

                entity.Property(e => e.EmailDeveloper)
                    .HasMaxLength(3000)
                    .IsUnicode(false);

                entity.Property(e => e.EmailFrom).HasMaxLength(3000);

                entity.Property(e => e.EmailHR)
                    .HasColumnName("EmailHR")
                    .HasMaxLength(3000);

                entity.Property(e => e.EmailPM)
                    .HasColumnName("EmailPM")
                    .HasMaxLength(3000);

                entity.Property(e => e.InductionDoc).HasMaxLength(200);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.pmid).HasColumnName("pmid");


                entity.Property(e => e.ProjectClosureEmail).HasMaxLength(3000);

                entity.Property(e => e.TimeSheetEmail).HasMaxLength(3000);
            });

            modelBuilder.Entity<Productivity>(entity =>
            {
                entity.HasKey(e => e.Pid);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<ProductLanding>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.ProductName).HasMaxLength(512);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProductLandingCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.ProductLandingModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ProductLandingScreenshot>(entity =>
            {
                entity.Property(e => e.ScreenshotUrl)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.HasOne(d => d.ProductLanding)
                    .WithMany(p => p.ProductLandingScreenshots)
                    .HasForeignKey(d => d.ProductLandingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductLandingScreenshot_ProductLandingId");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.AbroadPMUid).HasColumnName("AbroadPMUid");

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.BillingTeam).HasMaxLength(50);

                entity.Property(e => e.CRMProjectId).HasColumnName("CRMProjectId");

                entity.Property(e => e.IP)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.IsClosed)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
                entity.Property(e => e.IsCmmi).HasColumnName("isCMMI");

                entity.Property(e => e.IsManualTimeSheetAllowed).HasColumnName("isManualTimeSheetAllowed");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Notes).HasColumnType("ntext");

                entity.Property(e => e.PMUid).HasColumnName("PMUid");

                entity.Property(e => e.ProjectDetailsDoc).HasMaxLength(100);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.HasOne(d => d.AbroadPmu)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.AbroadPMUid)
                    .HasConstraintName("FK__Project__AbroadP__31AD415B");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_Project_Client");

                entity.HasOne(d => d.BucketModel)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.Model)
                    .HasConstraintName("FK_Project_BucketModel");
            });

            modelBuilder.Entity<ProjectAdditionalSupport>(entity =>
            {
                entity.Property(e => e.ApprovalDate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.TLId).HasColumnName("TLId");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProjectAdditionalSupportApproveByU)
                    .HasForeignKey(d => d.ApproveByUid);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectAdditionalSupports)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.ProjectAdditionalSupportRequestByU)
                    .HasForeignKey(d => d.RequestByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserLogin2)
                    .WithMany(p => p.ProjectAdditionalSupportTl)
                    .HasForeignKey(d => d.TLId)
                    .HasConstraintName("ProjectAdditionalSupport_UserLogin_TLId");
            });

            modelBuilder.Entity<ProjectAdditionalSupportUser>(entity =>
            {
                entity.HasKey(e => new { e.ProjectAdditionalSupportId, e.AssignedUid });

                entity.HasOne(d => d.AssignedU)
                    .WithMany(p => p.ProjectAdditionalSupportUser)
                    .HasForeignKey(d => d.AssignedUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectAdditionalSupportUser_AssignedUid");

                entity.HasOne(d => d.ProjectAdditionalSupport)
                    .WithMany(p => p.ProjectAdditionalSupportUser)
                    .HasForeignKey(d => d.ProjectAdditionalSupportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectAdditionalSupportUser_ProjectAdditionalSupportId");
            });

            modelBuilder.Entity<ProjectAuditPA>(entity =>
            {
                entity.ToTable("ProjectAuditPA");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<ProjectClose>(entity =>
            {
                entity.Property(e => e.ClientName).HasMaxLength(250);

                entity.Property(e => e.ClosureId)
                    .HasColumnName("ClosureID")
                    .HasMaxLength(50);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Crmid)
                    .HasColumnName("CRMID")
                    .HasMaxLength(50);

                entity.Property(e => e.Modified).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectClosure>(entity =>
            {
                entity.Property(e => e.Country).HasMaxLength(100);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.CRMStatus).HasColumnName("CRMStatus");

                entity.Property(e => e.CRMUpdated).HasColumnName("CRMUpdated");

                entity.Property(e => e.DateofClosing).HasColumnType("datetime");

                entity.Property(e => e.DeadResponseDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.NextStartDate).HasColumnType("datetime");

                entity.Property(e => e.OldCrmstatus).HasColumnName("OldCRMStatus");

                entity.Property(e => e.OtherActualDeveloper).HasMaxLength(500);

                entity.Property(e => e.PMID).HasColumnName("PMID");


                entity.Property(e => e.ProjectID).HasColumnName("ProjectID");

                entity.Property(e => e.ProjectLiveUrl).HasMaxLength(500);

                entity.Property(e => e.ProjectUrlAbsenseReason).HasMaxLength(500);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.Uid_BA).HasColumnName("Uid_BA");

                entity.Property(e => e.Uid_Dev).HasColumnName("Uid_Dev");

                entity.Property(e => e.Uid_TL).HasColumnName("Uid_TL");
                entity.Property(e => e.IsCovid19).HasColumnName("isCovid19");
                entity.Property(e => e.IsNewLeadGenerated).HasColumnName("IsNewLeadGenerated");

                entity.HasOne(d => d.UserLogin4)
                    .WithMany(p => p.ProjectClosureAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK_ProjectClosure_UserLogin3");

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.ProjectClosurePm)
                    .HasForeignKey(d => d.PMID)
                    .HasConstraintName("FK_ProjectClosure_UserLogin_PMId");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectClosures)
                    .HasForeignKey(d => d.ProjectID)
                    .HasConstraintName("FK_ProjectClosure_Project");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProjectClosureUidBaNavigation)
                    .HasForeignKey(d => d.Uid_BA)
                    .HasConstraintName("FK_ProjectClosure_UserLogin");

                entity.HasOne(d => d.UserLogin2)
                    .WithMany(p => p.ProjectClosureUidDevNavigation)
                    .HasForeignKey(d => d.Uid_Dev)
                    .HasConstraintName("FK_ProjectClosure_UserLogin1");

                entity.HasOne(d => d.UserLogin3)
                    .WithMany(p => p.ProjectClosureUidTlNavigation)
                    .HasForeignKey(d => d.Uid_TL)
                    .HasConstraintName("FK_ProjectClosure_UserLogin2");
            });
            modelBuilder.Entity<EstimatePriceCalculation>(entity =>
            {
                entity.Property(e => e.CrmleadId)
                    .HasColumnName("CRMLeadId")
                    .HasMaxLength(50);

                entity.Property(e => e.ExchangeRateAud).HasColumnName("ExchangeRateAUD");

                entity.Property(e => e.ExchangeRateUsd).HasColumnName("ExchangeRateUSD");

                entity.Property(e => e.TotalCost).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Country)
                   .WithMany(p => p.EstimatePriceCalculation)
                   .HasForeignKey(d => d.CountryId)
                   .HasConstraintName("EstimatePriceCalculation_EstimateCountry_CountryId");

                entity.HasOne(d => d.CreatedByU)
                    .WithMany(p => p.EstimatePriceCalculation)
                    .HasForeignKey(d => d.CreatedByUid)
                    .HasConstraintName("FK_EstimatePriceCalculation_UserLogin");

                entity.HasOne(d => d.EstimateModel)
                    .WithMany(p => p.EstimatePriceCalculation)
                    .HasForeignKey(d => d.EstimateModelId)
                    .HasConstraintName("EstimatePriceCalculation_EstimateModel_EstimateModelId");

                entity.HasOne(d => d.ModifiedByU)
                   .WithMany(p => p.EstimatePriceCalculationModifiedByU)
                   .HasForeignKey(d => d.ModifiedByUid)
                   .HasConstraintName("EstimatePriceCalculation_UserLogin_ModifiedByUid");
            });

            modelBuilder.Entity<EstimatePriceCalculationDetail>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.EstimateRoleExp)
                    .WithMany(p => p.EstimatePriceCalculationDetail)
                    .HasForeignKey(d => d.EstimateRoleExpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceCalculation_EstimateRoleExp");

                entity.HasOne(d => d.EstimateRole)
                    .WithMany(p => p.EstimatePriceCalculationDetail)
                    .HasForeignKey(d => d.EstimateRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceCalculation_EstimateRole");

                entity.HasOne(d => d.EstimateTechnology)
                .WithMany(p => p.EstimatePriceCalculationDetail)
                .HasForeignKey(d => d.EstimateTechnologyId)
                .HasConstraintName("EstimatePriceCalculationDetail_EstimateTechnology_EstimateTechnologyId");

                entity.HasOne(d => d.PriceCalculation)
                    .WithMany(p => p.EstimatePriceCalculationDetail)
                    .HasForeignKey(d => d.PriceCalculationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceCalculationDetail_PriceCalculation");
            });
            modelBuilder.Entity<ProjectClosureDetail>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.NextStartDate).HasColumnType("datetime");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProjectClosureDetail)
                    .HasForeignKey(d => d.AddedByUid)
                    .HasConstraintName("FK__ProjectCl__Added__382534C0");

                entity.HasOne(d => d.ProjectClosure)
                    .WithMany(p => p.ProjectClosureDetails)
                    .HasForeignKey(d => d.ProjectClosureId)
                    .HasConstraintName("FK__ProjectCl__Proje__15460CD7");
            });

            modelBuilder.Entity<ProjectClosureReview>(entity =>
            {
                entity.HasKey(e => e.ProjectClosureId);

                entity.Property(e => e.ProjectClosureId).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.NextStartDate).HasColumnType("date");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProjectClosureReviewCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.ProjectClosureReviewModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ProjectClosure)
                    .WithOne(p => p.ProjectClosureReview)
                    .HasForeignKey<ProjectClosureReview>(d => d.ProjectClosureId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Project_Department>(entity =>
            {
                entity.ToTable("Project_Department");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DeptID).HasColumnName("DeptID");

                entity.Property(e => e.ProjectID).HasColumnName("ProjectID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.ProjectDepartment)
                    .HasForeignKey(d => d.DeptID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Project_D__DeptI__1E6562BD");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Project_Department)
                    .HasForeignKey(d => d.ProjectID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Project_D__Proje__1D713E84");
            });

            modelBuilder.Entity<ProjectDeveloper>(entity =>
            {
                entity.Property(e => e.ProjectDeveloperId).HasColumnName("ProjectDeveloperID");

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.IP)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.Property(e => e.VD_id).HasColumnName("VD_id");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectDevelopers)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectDeveloper_Project");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProjectDevelopers)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_ProjectDeveloper_UserLogin");

                entity.HasOne(d => d.VirtualDeveloper)
                    .WithMany(p => p.ProjectDeveloper)
                    .HasForeignKey(d => d.VD_id)
                    .HasConstraintName("FK__ProjectDe__VD_id__5CACADF9");

                entity.HasOne(d => d.WorkStatusNavigation)
                    .WithMany(p => p.ProjectDeveloper)
                    .HasForeignKey(d => d.WorkStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectDeveloper_TypeMaster");
            });

            modelBuilder.Entity<ProjectDeveloperAddon>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.Uid, e.TransId });

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectDeveloperAddon)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectDeveloperAddon_Project");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.ProjectDeveloperAddon)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectDeveloperAddon_UserLogin");

                entity.HasOne(d => d.WorkRoleNavigation)
                    .WithMany(p => p.ProjectDeveloperAddonWorkRoleNavigation)
                    .HasForeignKey(d => d.WorkRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectDeveloperAddon_TypeMaster1");

                entity.HasOne(d => d.WorkStatusNavigation)
                    .WithMany(p => p.ProjectDeveloperAddonWorkStatusNavigation)
                    .HasForeignKey(d => d.WorkStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectDeveloperAddon_TypeMaster");
            });

            modelBuilder.Entity<ProjectInvoice>(entity =>
            {
                entity.Property(e => e.Country).HasMaxLength(100);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.CurrencyID).HasColumnName("CurrencyID");
                entity.Property(e => e.InvoiceAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InvoiceEndDate).HasColumnType("date");

                entity.Property(e => e.InvoiceNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.InvoiceStartDate).HasColumnType("date");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.PMID).HasColumnName("PMID");

                entity.Property(e => e.Uid_BA).HasColumnName("Uid_BA");

                entity.Property(e => e.Uid_TL).HasColumnName("Uid_TL");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.ProjectInvoice)
                    .HasForeignKey(d => d.CurrencyID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectInvoice_Currency");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectInvoice)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectIn__Proje__3B6BB5BF");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProjectInvoiceUidBaNavigation)
                    .HasForeignKey(d => d.Uid_BA)
                    .HasConstraintName("FK__ProjectIn__Uid_B__3C5FD9F8");

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.ProjectInvoiceUidTlNavigation)
                    .HasForeignKey(d => d.Uid_TL)
                    .HasConstraintName("FK__ProjectIn__Uid_T__3D53FE31");
            });

            modelBuilder.Entity<ProjectInvoiceComment>(entity =>
            {
                entity.Property(e => e.ChaseDate).HasColumnType("date");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.ProjectInvoice)
                    .WithMany(p => p.ProjectInvoiceComments)
                    .HasForeignKey(d => d.ProjectInvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectIn__Proje__4218B34E");
            });

            modelBuilder.Entity<ProjectLead>(entity =>
            {
                entity.HasKey(e => e.LeadId);

                entity.HasIndex(e => e.TitleCheckSum)
                    .HasName("UQ__ProjectL__508FDAD217E28260")
                    .IsUnique();

                entity.Property(e => e.AbroadPMID)
                    .HasColumnName("AbroadPMID")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AssignedDate).HasColumnType("datetime");

                entity.Property(e => e.Conclusion).HasMaxLength(1000);

                entity.Property(e => e.InitalRequirement).HasMaxLength(300);

                entity.Property(e => e.IP)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('127.0.0.1')");

                //entity.Property(e => e.IsNewClient)
                //    .IsRequired()
                //    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Isdelivered).HasDefaultValueSql("((1))");

                entity.Property(e => e.LeadCRMId)
                    .HasColumnName("LeadCRMId")
                    .HasMaxLength(20);

                entity.Property(e => e.MockupDocument).HasMaxLength(500);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NextChasedDate).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasColumnType("ntext");

                entity.Property(e => e.OtherDocument).HasMaxLength(500);

                entity.Property(e => e.PMID).HasColumnName("PMID");

                entity.Property(e => e.ProposalDocument).HasMaxLength(300);

                entity.Property(e => e.QuoteSubmittedDate).HasColumnType("datetime");

                entity.Property(e => e.StatusUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.Tag).HasMaxLength(100);
                entity.Property(e => e.IsCovid19).HasColumnName("isCovid19");
                entity.Property(e => e.ProjectClosureId).HasColumnName("ProjectClosureId");
                entity.Property(e => e.ConversionDate).HasColumnType("datetime");

                entity.Property(e => e.Technologies)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Wireframe_MockupsDoc)
                    .HasColumnName("Wireframe_MockupsDoc")
                    .HasMaxLength(300);

                entity.HasOne(d => d.AbroadPM)
                    .WithMany(p => p.ProjectLead)
                    .HasForeignKey(d => d.AbroadPMID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__Abroa__60283922");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProjectLeadCommunicator)
                    .HasForeignKey(d => d.CommunicatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__Commu__5F9E293D");

                entity.HasOne(d => d.LeadClient)
                    .WithMany(p => p.ProjectLead)
                    .HasForeignKey(d => d.LeadClientId)
                    .HasConstraintName("FK_LeadClientId");

                entity.HasOne(d => d.TypeMaster)
                    .WithMany(p => p.ProjectLead)
                    .HasForeignKey(d => d.LeadType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__LeadT__6A1BB7B0");

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.ProjectLeadOwner)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__Owner__5EAA0504");

                entity.HasOne(d => d.LeadStatu)
                    .WithMany(p => p.ProjectLead)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__Statu__618671AF");
            });

            modelBuilder.Entity<ProjectLeadArchive>(entity =>
            {
                entity.HasKey(e => e.LeadId);

                entity.Property(e => e.AbroadPmid).HasColumnName("AbroadPMID");

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AssignedDate).HasColumnType("datetime");

                entity.Property(e => e.Conclusion).HasMaxLength(1000);

                entity.Property(e => e.InitalRequirement).HasMaxLength(300);

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.Property(e => e.IsNewClient)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Notes).HasColumnType("ntext");

                entity.Property(e => e.QuoteSubmittedDate).HasColumnType("datetime");

                entity.Property(e => e.StatusUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.Technologies)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.AbroadPm)
                    .WithMany(p => p.ProjectLeadArchive)
                    .HasForeignKey(d => d.AbroadPmid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__Abroa__4159993F");

                entity.HasOne(d => d.Communicator)
                    .WithMany(p => p.ProjectLeadArchiveCommunicator)
                    .HasForeignKey(d => d.CommunicatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__Commu__40657506");

                entity.HasOne(d => d.LeadClient)
                    .WithMany(p => p.ProjectLeadArchive)
                    .HasForeignKey(d => d.LeadClientId)
                    .HasConstraintName("FK__ProjectLe__LeadC__3E7D2C94");

                entity.HasOne(d => d.LeadTypeNavigation)
                    .WithMany(p => p.ProjectLeadArchive)
                    .HasForeignKey(d => d.LeadType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__LeadT__424DBD78");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.ProjectLeadArchiveOwner)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__Owner__3F7150CD");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.ProjectLeadArchive)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__Statu__4341E1B1");
            });

            modelBuilder.Entity<ProjectLeadTech>(entity =>
            {
                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.ProjectLeadTeches)
                    .HasForeignKey(d => d.LeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectLeadTech_LeadId");

                entity.HasOne(d => d.Technology)
                    .WithMany(p => p.ProjectLeadTech)
                    .HasForeignKey(d => d.TechId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectLeadTech_TechId");
            });

            modelBuilder.Entity<ProjectLeadTechArchive>(entity =>
            {
                entity.HasKey(e => e.ProjectLeadTechId);

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.ProjectLeadTechArchive)
                    .HasForeignKey(d => d.LeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__LeadI__4DBF7024");

                entity.HasOne(d => d.Tech)
                    .WithMany(p => p.ProjectLeadTechArchive)
                    .HasForeignKey(d => d.TechId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectLe__TechI__4EB3945D");
            });

            modelBuilder.Entity<ProjectLesson>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProjectLessonCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ModifyByU)
                    .WithMany(p => p.ProjectLessonModifyByU)
                    .HasForeignKey(d => d.ModifyByUid);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectLesson)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ProjectLessonLearned>(entity =>
            {
                entity.HasKey(e => new { e.ProjectLessonId, e.ProjectLessonTopicId });

                entity.HasOne(d => d.ProjectLesson)
                    .WithMany(p => p.ProjectLessonLearneds)
                    .HasForeignKey(d => d.ProjectLessonId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ProjectLessonTopic)
                    .WithMany(p => p.ProjectLessonLearned)
                    .HasForeignKey(d => d.ProjectLessonTopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ProjectLessonTopic>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<ProjectNCLog>(entity =>
            {
                entity.ToTable("ProjectNCLog");

                entity.Property(e => e.AuditDate).HasColumnType("datetime");

                entity.Property(e => e.ClosedDate).HasColumnType("datetime");

                entity.Property(e => e.CompletedDate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.FollowUpDate).HasColumnType("date");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.ProjectAuditPAId).HasColumnName("ProjectAuditPAId");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ProjectNclogAuditeeU)
                    .HasForeignKey(d => d.AuditeeUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.ProjectNclogAuditorU)
                    .HasForeignKey(d => d.AuditorUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ProjectAuditPa)
                    .WithMany(p => p.ProjectNclog)
                    .HasForeignKey(d => d.ProjectAuditPAId);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectNclog)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ProjectPm>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.UserId });

                entity.ToTable("ProjectPM");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectPm)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_projectpm_project");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ProjectPm)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_projectpm_user");
            });

            modelBuilder.Entity<Project_Tech>(entity =>
            {
                entity.ToTable("Project_Tech");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ProjectID).HasColumnName("ProjectID");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Project_Tech)
                    .HasForeignKey(d => d.ProjectID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Project_Tech_Project");

                entity.HasOne(d => d.Technology)
                    .WithMany(p => p.ProjectTech)
                    .HasForeignKey(d => d.TechId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Project_Tech_Technology");
            });

            modelBuilder.Entity<Questions>(entity =>
            {
                entity.HasKey(e => e.Qid);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(500);
            });

            modelBuilder.Entity<ReadMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.Property(e => e.DateRead).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Forum)
                    .WithMany(p => p.ReadMessage)
                    .HasForeignKey(d => d.ForumId)
                    .HasConstraintName("FK_ReadMessage_Forums");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.ReadMessage)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_ReadMessage_UserLogin");
            });

            modelBuilder.Entity<Relationship>(entity =>
            {
                entity.HasKey(e => e.Rid);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<ReportBug>(entity =>
            {
                entity.HasKey(e => e.ReportId);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImageName).HasMaxLength(200);

                entity.Property(e => e.IP)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('127.0.0.1')");

                entity.Property(e => e.IsClosed).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PagePath).HasMaxLength(500);

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.Property(e => e.SectionDescription)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.SectionName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.ReportBug)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__ReportBug__UserI__0BB1B5A5");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.MenuAccess).HasMaxLength(500);

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });


           


            modelBuilder.Entity<SaturdayManagement>(entity =>
            {
                entity.HasIndex(e => new { e.Uid, e.SaturdayDt })
                    .HasName("unq_PresentDt")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddedOn).HasColumnType("datetime");

                entity.Property(e => e.LastProcessDt).HasColumnType("datetime");

                entity.Property(e => e.SaturdayDt).HasColumnType("datetime");

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.Property(e => e.UpdatedDt).HasColumnType("datetime");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.SaturdayManagement)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_SaturdayManagement_UserLogin");
            });

            modelBuilder.Entity<ServiceAuth>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ApiPass).HasMaxLength(50);

                entity.Property(e => e.Apikey)
                    .HasColumnName("APIKey")
                    .HasMaxLength(100);

                entity.Property(e => e.Modified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Sim>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Network)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.CreateByU)
                    .WithMany(p => p.SimCreateByU)
                    .HasForeignKey(d => d.CreateByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sim_UserLogin_CreatedByUid");

                entity.HasOne(d => d.ModifyByU)
                    .WithMany(p => p.SimModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SubProject>(entity =>
            {
                entity.HasKey(e => e.SubProjectId);

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.SubProjectName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValueSql("('')");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.SubProjects)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubProjects_SubProjects");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.TaskID)
                    .HasColumnName("TaskID")
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TaskEndDate).HasColumnType("datetime");

                entity.Property(e => e.TaskName).HasMaxLength(500);

                entity.Property(e => e.TaskStatusID).HasColumnName("TaskStatusID");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.Task)
                    .HasForeignKey(d => d.AddedUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Task__AddedUid__218BE82B");

                entity.HasOne(d => d.MomMeetingTask)
                   .WithMany(p => p.TaskNavigation)
                   .HasForeignKey(d => d.MomMeetingTaskId)
                   .HasConstraintName("FK__Task__MomMeeting__6E6C36D8");

                entity.HasOne(d => d.TaskStatu)
                    .WithMany(p => p.Task)
                    .HasForeignKey(d => d.TaskStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Task__TaskStatus__2097C3F2");
            });

            modelBuilder.Entity<CvsType>(entity =>
            {
                entity.HasKey(e => e.CvsId);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<SalesKitType>(entity =>
            {
                entity.HasKey(e => e.SalesKitId);
                entity.Property(e => e.DisplayName).HasMaxLength(200);
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<TaskAssignedTo>(entity =>
            {
                entity.HasKey(e => e.AssignedToTaskId);

                entity.Property(e => e.AssignedToTaskId)
                    .HasColumnName("AssignedToTaskID")
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TaskId)
                    .HasColumnName("TaskID")
                    .HasColumnType("numeric(18, 0)");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.TaskAssignedTo)
                    .HasForeignKey(d => d.AssignUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaskAssig__Assig__2DF1BF10");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskAssignedToes)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaskAssig__TaskI__2CFD9AD7");
                entity.HasOne(d => d.TaskStatus)
                   .WithMany(p => p.TaskAssignedTo)
                   .HasForeignKey(d => d.TaskStatusId);
            });

            modelBuilder.Entity<TaskComment>(entity =>
            {
                entity.Property(e => e.TaskCommentID)
                    .HasColumnName("TaskCommentID")
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AddedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TaskId)
                    .HasColumnName("TaskID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TaskStatusID).HasColumnName("TaskStatusID");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.TaskComment)
                    .HasForeignKey(d => d.AddedUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaskComme__Added__3D3402A0");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskComments)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaskComme__TaskI__32B6742D");

                entity.HasOne(d => d.TaskStatus)
                    .WithMany(p => p.TaskComment)
                    .HasForeignKey(d => d.TaskStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaskComme__TaskS__33AA9866");

            });

            modelBuilder.Entity<TaskStatu>(entity =>
            {
                entity.Property(e => e.TaskStatusId)
                    .HasColumnName("TaskStatusID")
                    .ValueGeneratedNever();

                entity.Property(e => e.TaskStatus)
                    .HasColumnName("TaskStatus")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Technology>(entity =>
            {
                entity.HasKey(e => e.TechId);

                entity.Property(e => e.AddDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.Alias).HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<TeamHierarchy>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Pmuid).HasColumnName("PMUid");

                entity.HasOne(d => d.UserLoginDetails)
                    .WithMany(p => p.TeamHierarchy)
                    .HasForeignKey(d => d.TlId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamHierarchy_UserLogin");
            });

            modelBuilder.Entity<TypeMaster>(entity =>
            {
                entity.HasKey(e => e.TypeId);

                entity.Property(e => e.TypeId).ValueGeneratedNever();

                entity.Property(e => e.TypeGroup)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<TdsassesmentYear>(entity =>
            {
                entity.HasKey(e => e.AssesmentYearId)
                    .HasName("PK_AssesmentYear");

                entity.ToTable("TDSAssesmentYear");

                entity.Property(e => e.YearRange)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TdsdeductionDoc>(entity =>
            {
                entity.HasKey(e => e.DeductionDocId)
                    .HasName("PK_DeductionDoc");

                entity.ToTable("TDSDeductionDoc");

                entity.Property(e => e.DeductionDocId)
                    .HasColumnName("DeductionDocID");
                   

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmpDeductionId).HasColumnName("EmpDeductionID");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Files)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Remark).HasMaxLength(250);
                entity.Property(e => e.Uid).HasColumnName("UID");
                entity.HasOne(d => d.EmpDeduction);

                entity.HasOne(d => d.EmpDeduction)
               .WithMany(p => p.TdsdeductionDoc)
               .HasForeignKey(d => d.EmpDeductionId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_TDSDeductionDoc_TDSEmpDeduction");
            });

            modelBuilder.Entity<TdsdeductionType>(entity =>
            {
                entity.HasKey(e => e.DeductionTypeId)
                    .HasName("PK_DeductionType");

                entity.ToTable("TDSDeductionType");

                entity.Property(e => e.DeductionTypeId)
                    .HasColumnName("DeductionTypeID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DeductionTypeName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.DeductionType)
                    .WithOne(p => p.TdsdeductionType)
                    .HasForeignKey<TdsdeductionType>(d => d.DeductionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DeductionType_Type");
            });

            modelBuilder.Entity<TdsempDeduction>(entity =>
            {
                entity.HasKey(e => e.EmpDeductionId)
                    .HasName("PK_EmpDeduction");

                entity.ToTable("TDSEmpDeduction");

                entity.Property(e => e.EmpDeductionId).HasColumnName("EmpDeductionID");

                entity.Property(e => e.ClaimedByEmployee).HasColumnType("money");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeductionTypeId).HasColumnName("DeductionTypeID");

                entity.Property(e => e.EmployeeRemark).HasMaxLength(500);

                entity.Property(e => e.EmployerRemark).HasMaxLength(500);

                entity.Property(e => e.GivenByEmployer).HasColumnType("money");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifyByAc).HasColumnName("ModifyByAC");

                entity.Property(e => e.ModifyDateAc).HasColumnType("datetime");

                entity.Property(e => e.ModifyDateEmp).HasColumnType("datetime");

                entity.Property(e => e.OtherDeduction).HasMaxLength(250);

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.Property(e => e.PanNumberByEmployee).HasMaxLength(40);

                entity.Property(e => e.Is13Ahradedution).HasColumnName("Is13AHRADEDUTION");

                entity.Property(e => e.Is24Bhouseloan).HasColumnName("Is24BHOUSELOAN");

                entity.Property(e => e.IsLockUnlock).HasColumnName("IsLockUnlock");

                entity.HasOne(d => d.AssesmentYear)
                    .WithMany(p => p.TdsempDeduction)
                    .HasForeignKey(d => d.AssesmentYearId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TDSEmpDeduction_TDSAssesmentYear");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TdsempDeductionCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TDSEmpDeduction_UserLogin1");

                entity.HasOne(d => d.DeductionType)
                    .WithMany(p => p.TdsempDeduction)
                    .HasForeignKey(d => d.DeductionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TDSEmpDeduction_TDSDeductionType");

                entity.HasOne(d => d.ModifyByAcNavigation)
                    .WithMany(p => p.TdsempDeductionModifyByAcNavigation)
                    .HasForeignKey(d => d.ModifyByAc)
                    .HasConstraintName("FK_TDSEmpDeduction_UserLogin3");

                entity.HasOne(d => d.ModifyByEmpNavigation)
                    .WithMany(p => p.TdsempDeductionModifyByEmpNavigation)
                    .HasForeignKey(d => d.ModifyByEmp)
                    .HasConstraintName("FK_TDSEmpDeduction_UserLogin2");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.TdsempDeduction)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_TDSEmpDeduction_TDSType");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.TdsempDeductionU)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TDSEmpDeduction_UserLogin");
            });

            modelBuilder.Entity<Tdstype>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK_Type");

                entity.ToTable("TDSType");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(250);
            });


            modelBuilder.Entity<UserActivity>(entity =>
            {
                entity.HasKey(e => e.ActivityID);

                entity.Property(e => e.ActivityID).HasColumnName("ActivityID");

                entity.Property(e => e.Comment).HasMaxLength(2000);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.DateModify).HasColumnType("datetime");

                entity.Property(e => e.ProjectName).HasMaxLength(500);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.UserActivity)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_UA_Project_ID");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.UserActivities)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_UserActivityCheck_UserLogin");
            });

            modelBuilder.Entity<UserActivityManageProject>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.ProjectStatus)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.UserActivityManageProject)
                    .HasForeignKey(d => d.ActivityId)
                    .HasConstraintName("FK__UserActiv__Activ__275A9B71");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.UserActivityManageProject)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__UserActiv__Proje__284EBFAA");
            });

            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasKey(e => e.ActivityLogId);

                entity.Property(e => e.ActivityLogId).HasColumnName("ActivityLogID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.ProjectName).HasMaxLength(500);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.UserActivityLog)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_UAL_Project_ID");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.UserActivityLog)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_UserActivityLog_UserLogin");
            });

            modelBuilder.Entity<UserLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Login).HasColumnType("datetime");

                entity.Property(e => e.Logout).HasColumnType("datetime");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.UserLog)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_UserLog_UserLog");
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

                entity.Property(e => e.IsFromDbdt).HasColumnName("IsFromDBDT");

                entity.Property(e => e.AlternativeNumber).HasMaxLength(50);

                entity.Property(e => e.ApiPassword).HasMaxLength(50);

                entity.Property(e => e.CRMUserId).HasColumnName("CRMUserId");

                entity.Property(e => e.DOB)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.EmailOffice).HasMaxLength(200);

                entity.Property(e => e.EmailPersonal).HasMaxLength(200);

                entity.Property(e => e.EmpCode).HasMaxLength(50);

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.HRMId).HasColumnName("HRMId");
                entity.Property(e => e.AttendenceId).HasColumnName("AttendenceId");

                entity.Property(e => e.IP)
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('0.0.0.0')");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsInterestedPffaccount).HasColumnName("IsInterestedPFFAccount");

                entity.Property(e => e.IsSPEG).HasColumnName("IsSPEG");

                entity.Property(e => e.JobTitle).HasMaxLength(200);

                entity.Property(e => e.JoinedDate).HasColumnType("datetime");

                entity.Property(e => e.MarraigeDate).HasColumnType("date");

                entity.Property(e => e.MobileNumber).HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.PanNumber).HasMaxLength(20);

                entity.Property(e => e.PassportNumber).HasMaxLength(20);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.PasswordBackUp).HasMaxLength(100);

                entity.Property(e => e.PMUid).HasColumnName("PMUid");

                entity.Property(e => e.SkypeId).HasMaxLength(200);

                entity.Property(e => e.Title).HasMaxLength(20);

                entity.Property(e => e.TLId).HasColumnName("TLId");

                entity.Property(e => e.UserName).HasMaxLength(200);
                entity.Property(e => e.OtherTechnology).HasMaxLength(250);
                entity.Property(e => e.RelievingDate).HasColumnType("datetime");

                entity.Property(e => e.ResignationDate).HasColumnType("datetime");

                entity.HasOne(d => d.BloodGroup)
                    .WithMany(p => p.UserLogin)
                    .HasForeignKey(d => d.BloodGroupId)
                    .HasConstraintName("FK__UserLogin__Blood__521A10ED");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.UserLogin)
                    .HasForeignKey(d => d.DeptId)
                    .HasConstraintName("FK_UserLogin_Department");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserLogins)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_UserLogin_Role");

                entity.HasOne(d => d.Pmu)
                   .WithMany(p => p.InversePmu)
                   .HasForeignKey(d => d.PMUid)
                   .HasConstraintName("FK__PMUid__UID");
            });

            modelBuilder.Entity<User_Tech>(entity =>
            {
                entity.ToTable("User_Tech");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Technology)
                    .WithMany(p => p.UserTech)
                    .HasForeignKey(d => d.TechId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Tech_Project");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.User_Tech)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Tech_UserLogin");
            });

            modelBuilder.Entity<UserTimeSheet>(entity =>
            {
                entity.Property(e => e.UserTimeSheetID)
                    .HasColumnName("UserTimeSheetID")
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.ProjectID).HasColumnName("ProjectID");

                entity.Property(e => e.ReviewedDate).HasColumnType("datetime");

                entity.Property(e => e.UID).HasColumnName("UID");

                entity.Property(e => e.VirtualDeveloper_id).HasColumnName("VirtualDeveloper_id");

                entity.Property(e => e.IsFillByPMS).HasColumnName("IsFillByPMS");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.UserTimeSheet)
                    .HasForeignKey(d => d.ProjectID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Project_UserTimeSheet");

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.UserTimeSheet)
                    .HasForeignKey(d => d.ReviewedByUid)
                    .HasConstraintName("FK__UserTimeS__Revie__4B6D135E");

                entity.HasOne(d => d.UserLogin1)
                    .WithMany(p => p.UserTimeSheets1)
                    .HasForeignKey(d => d.UID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLogin_UserTimeSheet");

                entity.HasOne(d => d.VirtualDeveloper)
                    .WithMany(p => p.UserTimeSheet)
                    .HasForeignKey(d => d.VirtualDeveloper_id)
                    .HasConstraintName("FK_VirtualDeveloper_UserTimeSheet");
            });

            modelBuilder.Entity<VirtualDeveloper>(entity =>
            {
                entity.Property(e => e.VirtualDeveloper_ID).HasColumnName("VirtualDeveloper_ID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.emailid)
                    .HasColumnName("emailid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.isactive).HasColumnName("isactive");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PMUid).HasColumnName("PMUid");

                entity.Property(e => e.SkypeId)
                    .HasColumnName("Skype_id")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VirtualDeveloper_Name)
                    .HasColumnName("VirtualDeveloper_Name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Momdocument>(entity =>
            {
                entity.ToTable("MOMDocument");

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentPath).HasMaxLength(500);
            });
            modelBuilder.Entity<ProjectClientFeedback>(entity =>
            {

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.BusinessDescription).HasMaxLength(500);

                entity.Property(e => e.ClientName).HasMaxLength(250);

                entity.Property(e => e.CommentDate).HasColumnType("datetime");

                entity.Property(e => e.CompanyName).HasMaxLength(300);

                entity.Property(e => e.Status).HasMaxLength(200);

                entity.Property(e => e.CrmfeedbackId).HasColumnName("CRMFeedbackId");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.WebUrl).HasMaxLength(256);

                entity.Property(e => e.WebsiteName).HasMaxLength(250);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectClientFeedback)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.ProjectClientFeedback)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("ProjectClientFeedback_Userlogin_AddedBy");
            });
            modelBuilder.Entity<ProjectClientFeedbackDocument>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentPath).HasMaxLength(500);

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.ProjectClientFeedbackDocument)
                    .HasForeignKey(d => d.FeedbackId)
                    .HasConstraintName("ProjectClientFeedbackDocument_ProjectClientFeedback_FeedbackId");
            });

            modelBuilder.Entity<MomMeetingTaskDocument>(entity =>
            {
                entity.ToTable("MomMeetingTaskDocument");

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentPath).HasMaxLength(500);
            });

            modelBuilder.Entity<Library>(entity =>
            {
                entity.HasIndex(e => e.KeyId)
                    .HasName("UK_KeyId")
                    .IsUnique();
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");
                entity.Property(e => e.EstimatedHours).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.IntegrationHours).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ReDevelopmentHours).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);
                entity.Property(e => e.BannerImage).HasMaxLength(1000);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(20);

                entity.Property(e => e.IsNda).HasColumnName("IsNDA");

                entity.Property(e => e.KeyId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LibraryCreatedDate).HasColumnType("datetime");

                entity.Property(e => e.OtherIndustry).HasMaxLength(100);

                entity.Property(e => e.OtherTechnology).HasMaxLength(100);

                entity.Property(e => e.OtherTechnologyParent).HasMaxLength(100);
                entity.Property(e => e.UidBa).HasColumnName("UidBA");

                entity.Property(e => e.UidTl).HasColumnName("UidTL");

                entity.Property(e => e.LiveUrl)
                    .HasColumnName("LiveURL")
                    .HasMaxLength(300);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.LibraryAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ModifyByU)
                    .WithMany(p => p.LibraryModifyByU)
                    .HasForeignKey(d => d.ModifyByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.AuthorU)
                    .WithMany(p => p.LibraryAuthorU)
                    .HasForeignKey(d => d.AuthorUid);

                entity.HasOne(d => d.UidBaNavigation)
                    .WithMany(p => p.LibraryUidBaNavigation)
                    .HasForeignKey(d => d.UidBa);

                entity.HasOne(d => d.UidTlNavigation)
                    .WithMany(p => p.LibraryUidTlNavigation)
                    .HasForeignKey(d => d.UidTl);

                entity.HasOne(d => d.Cvs)
                    .WithMany(p => p.Library)
                    .HasForeignKey(d => d.CvsId)
                    .HasConstraintName("FK_libraryCVsId");

                entity.HasOne(d => d.SalesKit)
                    .WithMany(p => p.Library)
                    .HasForeignKey(d => d.SalesKitId)
                    .HasConstraintName("FK_LibrarySalesKitId");
            });

            modelBuilder.Entity<LibraryFile>(entity =>
            {
                entity.HasIndex(e => e.SrNo)
                    .HasName("UQ__LibraryF__C3A4D3CD4D9552F2")
                    .IsUnique();

                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.FileId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.PsdfilePath)
                    .HasColumnName("PSDFilePath")
                    .HasMaxLength(1000);

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.SrNo).HasColumnName("SrNO");

                entity.Property(e => e.KeyId).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.LibraryFileType)
                    .WithMany(p => p.LibraryFile)
                    .HasForeignKey(d => d.LibraryFileTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Library)
                    .WithMany(p => p.LibraryFile)
                    .HasForeignKey(d => d.LibraryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LibraryLayoutType)
                    .WithMany(p => p.LibraryFile)
                    .HasForeignKey(d => d.LibraryLayoutTypeId);
            });

            modelBuilder.Entity<LibraryFileType>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");
                entity.Property(e => e.Extension).HasMaxLength(20);
                entity.Property(e => e.Icon).HasMaxLength(1000);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

            });

            modelBuilder.Entity<LibraryIndustry>(entity =>
            {
                entity.HasKey(e => new { e.LibraryId, e.IndustryId });

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.LibraryIndustry)
                    .HasForeignKey(d => d.IndustryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Library)
                    .WithMany(p => p.LibraryIndustry)
                    .HasForeignKey(d => d.LibraryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LibraryLayoutType>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);
            });

            modelBuilder.Entity<LibraryLayoutTypeMapping>(entity =>
            {
                entity.HasKey(e => new { e.LibraryId, e.LibraryLayoutTypeId });

                entity.HasOne(d => d.Library)
                    .WithMany(p => p.LibraryLayoutTypeMapping)
                    .HasForeignKey(d => d.LibraryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LibraryLayoutType)
                    .WithMany(p => p.LibraryLayoutTypeMapping)
                    .HasForeignKey(d => d.LibraryLayoutTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LibraryTechnology>(entity =>
            {
                entity.HasKey(e => new { e.LibraryId, e.TechnologyId });

                entity.HasOne(d => d.Library)
                    .WithMany(p => p.LibraryTechnology)
                    .HasForeignKey(d => d.LibraryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Technology)
                    .WithMany(p => p.LibraryTechnology)
                    .HasForeignKey(d => d.TechnologyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LibraryTechnology_DomainType_TechnologyId");
            });

            modelBuilder.Entity<LibrarySearch>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(20);

                //entity.Property(e => e.IsNda).HasColumnName("IsNDA");

                entity.Property(e => e.SearchDate).HasColumnType("datetime");
                entity.HasOne(d => d.Key)
                    .WithMany(p => p.LibrarySearch)
                    .HasPrincipalKey(p => p.KeyId)
                    .HasForeignKey(d => d.KeyId);
            });

            modelBuilder.Entity<LibrarySearchIndustry>(entity =>
            {
                entity.HasKey(e => new { e.LibrarySearchId, e.DomainId });

                entity.HasOne(d => d.Domain)
                    .WithMany(p => p.LibrarySearchIndustry)
                    .HasForeignKey(d => d.DomainId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LibrarySearch)
                    .WithMany(p => p.LibrarySearchIndustry)
                    .HasForeignKey(d => d.LibrarySearchId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LibrarySearchTechnology>(entity =>
            {
                entity.HasKey(e => new { e.LibrarySearchId, e.TechnologyId });

                entity.HasOne(d => d.LibrarySearch)
                    .WithMany(p => p.LibrarySearchTechnology)
                    .HasForeignKey(d => d.LibrarySearchId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Technology)
                    .WithMany(p => p.LibrarySearchTechnology)
                    .HasForeignKey(d => d.TechnologyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
            modelBuilder.Entity<LibraryDownloadHistory>(entity =>
            {
                entity.Property(e => e.DownloadOn).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.HasOne(d => d.DownloadByNavigation)
                    .WithMany(p => p.LibraryDownloadHistory)
                    .HasForeignKey(d => d.DownloadBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LibraryFile)
                    .WithMany(p => p.LibraryDownloadHistory)
                    .HasForeignKey(d => d.LibraryFileId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LibraryFileType)
                    .WithMany(p => p.LibraryDownloadHistory)
                    .HasForeignKey(d => d.LibraryFileTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Library)
                    .WithMany(p => p.LibraryDownloadHistory)
                    .HasForeignKey(d => d.LibraryId);
            });

            modelBuilder.Entity<LibraryDownloadPermission>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.AllowedDownloadByNavigation)
                    .WithMany(p => p.LibraryDownloadPermissionAllowedDownloadByNavigation)
                    .HasForeignKey(d => d.AllowedDownloadBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LibraryFileType)
                    .WithMany(p => p.LibraryDownloadPermission)
                    .HasForeignKey(d => d.LibraryFileTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.LibraryDownloadPermission)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.UserLogin)
                    .WithMany(p => p.LibraryDownloadPermissionUserLogin)
                    .HasForeignKey(d => d.UserLoginId);
            });

            modelBuilder.Entity<LibrarySearchLayoutTypeMapping>(entity =>
            {
                entity.HasKey(e => new { e.LibrarySearchId, e.LayoutTypeId })
                    .HasName("PK_LibrarySearchLibraryLayoutType");

                entity.HasOne(d => d.LayoutType)
                    .WithMany(p => p.LibrarySearchLayoutTypeMapping)
                    .HasForeignKey(d => d.LayoutTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LibrarySearch)
                    .WithMany(p => p.LibrarySearchLayoutTypeMapping)
                    .HasForeignKey(d => d.LibrarySearchId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LibrarySearchComponent>(entity =>
            {
                entity.HasKey(e => new { e.LibrarySearchId, e.ComponentTypeId });

                entity.HasOne(d => d.ComponentType)
                    .WithMany(p => p.LibrarySearchComponent)
                    .HasForeignKey(d => d.ComponentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LibrarySearch)
                    .WithMany(p => p.LibrarySearchComponent)
                    .HasForeignKey(d => d.LibrarySearchId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserExitProcess>(entity =>
            {
                entity.Property(e => e.EmpPmuid).HasColumnName("EmpPMUid");

                entity.Property(e => e.IsLfprofileResetted).HasColumnName("IsLFProfileResetted");

                entity.Property(e => e.RehireComment).HasMaxLength(500);

                entity.Property(e => e.VoluntaryComment).HasMaxLength(500);

                entity.HasOne(d => d.EmpU)
                    .WithMany(p => p.UserExitProcess)
                    .HasForeignKey(d => d.EmpUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLogin_UserExitProcess");
            });

            modelBuilder.Entity<NocMaster>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserNoc>(entity =>
            {
                entity.HasOne(d => d.Noc)
                    .WithMany(p => p.UserNoc)
                    .HasForeignKey(d => d.Nocid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NocMaster_UserNoc");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.UserNoc)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLogin_UserNoc");
            });

            modelBuilder.Entity<OrgImprovement>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ImprovementDate).HasColumnType("date");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.AddedByU)
                    .WithMany(p => p.OrgImprovementAddedByU)
                    .HasForeignKey(d => d.AddedByUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Improvement_UserLogin_AddedByUid");

                entity.HasOne(d => d.EmployeeU)
                    .WithMany(p => p.OrgImprovementEmployeeU)
                    .HasForeignKey(d => d.EmployeeUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Improvement_UserLogin_EmployeeUid");
            });
            modelBuilder.Entity<SelfDeclaration>(entity =>
            {
                entity.Property(e => e.DeclarationName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.EmailPersonal).HasMaxLength(200);

                entity.Property(e => e.EmpCode).HasMaxLength(50);

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.JoiningDate).HasColumnType("datetime");

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.MobileNumber).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Purpose).HasMaxLength(200);

                entity.Property(e => e.SymptomsEndDate).HasColumnType("datetime");

                entity.Property(e => e.SymptomsStartDate).HasColumnType("datetime");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.SelfDeclaration)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SelfDeclaration_UserLogin_Uid");
            });
            modelBuilder.Query<GetMonthTimesheets_Result>();
            modelBuilder.Query<GetTimesheetsByTimesheetIds_Result>();
            modelBuilder.Query<AllTeamStatusReportGraph_Result>();
            modelBuilder.Query<TeamStatusReportGraph_Result>();

            modelBuilder.Query<TeamStatusReportGraphDetail_Result>();

            modelBuilder.Query<GETMOMListByMeetingMasterID_Result>();

            modelBuilder.Entity<LibraryTemplate>(entity =>
            {
                entity.HasKey(e => new { e.LibraryId, e.LibraryTemplateTypeId });
            });

            modelBuilder.Entity<LibraryTemplateType>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(300)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VirtualDeveloperCategory>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Pmuid).HasColumnName("PMUid");

                entity.Property(e => e.VirtualDeveloperName)
                    .HasColumnName("VirtualDeveloper_Name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProjectLeadIndustry>(entity =>
            {
                entity.HasKey(e => new { e.ProjectLeadId, e.IndustryId });

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.ProjectLeadIndustry)
                    .HasForeignKey(d => d.IndustryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ProjectLead)
                    .WithMany(p => p.ProjectLeadIndustry)
                    .HasForeignKey(d => d.ProjectLeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectLeadIndustry_ProjectLead_LeadId");
            });

            modelBuilder.Entity<EstimateDocumentIndustry>(entity =>
            {
                entity.HasKey(e => new { e.EstimateDocumentId, e.IndustryId });

                entity.HasOne(d => d.EstimateDocument)
                    .WithMany(p => p.EstimateDocumentIndustry)
                    .HasForeignKey(d => d.EstimateDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.EstimateDocumentIndustry)
                    .HasForeignKey(d => d.IndustryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LessonLearnt>(entity =>
            {
                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.LessonLearnt)
                    .HasForeignKey(d => d.CreatedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LessonLea__Creat__5CF79213");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.LessonLearnt)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__LessonLea__Proje__5C036DDA");
            });

            modelBuilder.Entity<EstimateHourFileNameType>(entity =>
            {
                entity.Property(e => e.AddDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.UploadFileName).HasMaxLength(2000);
            });

            modelBuilder.Entity<EstimateRole>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EstimateRoleExp>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.EstimateRole)
                    .WithMany(p => p.EstimateRoleExp)
                    .HasForeignKey(d => d.EstimateRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EstimateRole_EstimateRoleExp_EstimateRoleId");
            });

            modelBuilder.Entity<EstimateRoleTechnoloyPrice>(entity =>
            {
                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.EstimateRoleExp)
                    .WithMany(p => p.EstimateRoleTechnoloyPrice)
                    .HasForeignKey(d => d.EstimateRoleExpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EstimateRoleTechnoloyPrice_EstimateRoleExp_EstimateRoleExpId");

                entity.HasOne(d => d.EstimateTechnology)
                   .WithMany(p => p.EstimateRoleTechnoloyPrice)
                   .HasForeignKey(d => d.EstimateTechnologyId)
                   .HasConstraintName("EstimateRoleTechnoloyPrice_EstimateTechnology_EstimateTechnologyId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<EstimateTechnology>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstimateModel>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<UserDocument>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentPath).HasMaxLength(500);

                entity.Property(e => e.Uid).HasColumnName("UId");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.UserDocument)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__UserDocumen__UId__3AF77FCA");
            });

            modelBuilder.Entity<Wfhactivity>(entity =>
            {
                entity.HasKey(e => e.Wfhid)
                    .HasName("PK_WFHStatus");

                entity.ToTable("WFHActivity");

                entity.Property(e => e.Wfhid).HasColumnName("WFHId");

                entity.Property(e => e.AnyComment).HasColumnType("ntext");

                entity.Property(e => e.Comment).HasColumnType("ntext");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.DateModify).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.Wfhcategory).HasColumnName("WFHCategory");

                entity.HasOne(d => d.ApprovedBy)
                    .WithMany(p => p.WfhactivityApprovedBy)
                    .HasForeignKey(d => d.ApprovedById)
                    .HasConstraintName("FK_WFHActivity_UserLogin1");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.WfhactivityU)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WFHActivity_UserLogin");

                entity.HasOne(d => d.WfhcategoryNavigation)
                    .WithMany(p => p.Wfhactivity)
                    .HasForeignKey(d => d.Wfhcategory)
                    .HasConstraintName("FK__WFHActivi__WFHCa__033D368C");
            });

            modelBuilder.Entity<WfhtypesMaster>(entity =>
            {
                entity.ToTable("WFHTypesMaster");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(20);
            });
            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<DocumentDepartment>(entity =>
            {
                entity.HasOne(d => d.Document)
                    .WithMany(p => p.DocumentDepartment)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DocumentD__Docum__3999473D");
            });

            modelBuilder.Entity<StudyDocumentFiles>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Filename)
                    .HasColumnName("filename")
                    .HasMaxLength(100)
                    .IsUnicode(false);
                
                entity.Property(e => e.Displayname)
                    .HasColumnName("displayname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Keyid)
                    .HasColumnName("keyid")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Studydocumentid).HasColumnName("studydocumentid");

                entity.HasOne(d => d.Studydocument)
                    .WithMany(p => p.StudyDocumentFiles)
                    .HasForeignKey(d => d.Studydocumentid)
                    .HasConstraintName("FK_StudyDocumentFiles_studydocumentid");
            });

            modelBuilder.Entity<StudyDocuments>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Addedby).HasColumnName("addedby");

                entity.Property(e => e.Addeddate)
                    .HasColumnName("addeddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.Isdelete).HasColumnName("isdelete");

                entity.Property(e => e.Keyid)
                    .HasColumnName("keyid")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Technologyid).HasColumnName("technologyid");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Updatedby).HasColumnName("updatedby");

                entity.Property(e => e.Updateddate)
                    .HasColumnName("updateddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Isapproved).HasColumnName("isapproved");

                entity.Property(e => e.Unapprovedreson)
                    .HasColumnName("unapprovedreson")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.AddedbyNavigation)
                    .WithMany(p => p.StudyDocumentsAddedbyNavigation)
                    .HasForeignKey(d => d.Addedby)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudyDocuments_addedby");

                entity.HasOne(d => d.UpdatedbyNavigation)
                    .WithMany(p => p.StudyDocumentsUpdatedbyNavigation)
                    .HasForeignKey(d => d.Updatedby)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudyDocuments_updateby");
            });

            modelBuilder.Entity<StudyDocumentsPermissions>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Studydocumentsid).HasColumnName("studydocumentsid");

                entity.Property(e => e.Userid).HasColumnName("userid");

                entity.HasOne(d => d.Studydocuments)
                    .WithMany(p => p.StudyDocumentsPermissions)
                    .HasForeignKey(d => d.Studydocumentsid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudyDocumentsPermissions_studydocumentsid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StudyDocumentsPermissions)
                    .HasForeignKey(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudyDocumentsPermissions_userid");
            });

            modelBuilder.Entity<RequestedStudyDocuments>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Addeddate)
                    .HasColumnName("addeddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Requestedby).HasColumnName("requestedby");

                entity.Property(e => e.Studydocumentid).HasColumnName("studydocumentid");

                entity.HasOne(d => d.RequestedbyNavigation)
                    .WithMany(p => p.RequestedStudyDocuments)
                    .HasForeignKey(d => d.Requestedby)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestedStudyDocuments_requestedby");

                entity.HasOne(d => d.Studydocument)
                    .WithMany(p => p.RequestedStudyDocuments)
                    .HasForeignKey(d => d.Studydocumentid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestedStudyDocuments_studydocumentid");
            });
            modelBuilder.Entity<CvBuilderEducation>(entity =>
            {
                entity.ToTable("CvBuilder_Education");

                entity.Property(e => e.CvbuilderId).HasColumnName("CVBuilderId");

                entity.Property(e => e.Title).HasMaxLength(1000);

                entity.HasOne(d => d.Cvbuilder)
                    .WithMany(p => p.CvBuilderEducation)
                    .HasForeignKey(d => d.CvbuilderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CvBuilder__CVBui__21ABE3BC");
            });

            modelBuilder.Entity<Cvbuilder>(entity =>
            {
                entity.ToTable("CVBuilder");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.OtherIndustry).HasMaxLength(100);

                entity.Property(e => e.OtherTechnology).HasMaxLength(100);

                entity.Property(e => e.OtherTechnologyParent).HasMaxLength(100);
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Cvbuilder)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CVBuilder__UserI__135DC465");
            });

            modelBuilder.Entity<CvbuilderCertifications>(entity =>
            {
                entity.ToTable("CVBuilder_Certifications");

                entity.Property(e => e.CertificationName).HasMaxLength(1000);

                entity.Property(e => e.CertificationNumber).HasMaxLength(500);

                entity.Property(e => e.CvbuilderId).HasColumnName("CVBuilderId");

                entity.HasOne(d => d.Cvbuilder)
                    .WithMany(p => p.CvbuilderCertifications)
                    .HasForeignKey(d => d.CvbuilderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CVBuilder__CVBui__1FC39B4A");
            });

            modelBuilder.Entity<CvbuilderCoreCompetencies>(entity =>
            {
                entity.ToTable("CVBuilder_CoreCompetencies");

                entity.Property(e => e.CvbuilderId).HasColumnName("CVBuilderId");

                entity.Property(e => e.Title).HasMaxLength(1000);

                entity.HasOne(d => d.Cvbuilder)
                    .WithMany(p => p.CvbuilderCoreCompetencies)
                    .HasForeignKey(d => d.CvbuilderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CVBuilder__CVBui__20B7BF83");
            });

            modelBuilder.Entity<CvbuilderPreviousExperience>(entity =>
            {
                entity.ToTable("CVBuilder_PreviousExperience");

                entity.Property(e => e.CvbuilderId).HasColumnName("CVBuilderId");

                entity.Property(e => e.Designation).HasMaxLength(500);

                entity.Property(e => e.FromDate).HasMaxLength(50);

                entity.Property(e => e.OrganizationName).HasMaxLength(500);

                entity.Property(e => e.ToDate).HasMaxLength(50);

                entity.HasOne(d => d.Cvbuilder)
                    .WithMany(p => p.CvbuilderPreviousExperience)
                    .HasForeignKey(d => d.CvbuilderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CVBuilder__CVBui__22A007F5");
            });
            modelBuilder.Entity<ExperienceType>(entity =>
            {
                entity.Property(e => e.Experience).HasMaxLength(50);
            });
            modelBuilder.Entity<CvbuilderIndustry>(entity =>
            {
                entity.ToTable("CVBuilder_Industry");

                entity.Property(e => e.CvbuilderId).HasColumnName("CVBuilderId");

                entity.HasOne(d => d.Cvbuilder)
                    .WithMany(p => p.CvbuilderIndustry)
                    .HasForeignKey(d => d.CvbuilderId)
                    .HasConstraintName("FK__CVBuilder__CVBui__4AADF94F");
                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.CvbuilderIndustry)
                    .HasForeignKey(d => d.IndustryId)
                    .HasConstraintName("FK__CVBuilder__Indus__4BA21D88");
            });
            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.Property(e => e.ActionTaken).HasDefaultValueSql("((0))");

                entity.Property(e => e.ActionTakenOn).HasColumnType("date");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.ReminderDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ReminderUser>(entity =>
            {
                entity.HasOne(d => d.Reminder)
                    .WithMany(p => p.ReminderUser)
                    .HasForeignKey(d => d.ReminderId)
                    .HasConstraintName("FK_Reminder_ReminderUser");
            });

            modelBuilder.Entity<CvbuilderTechnology>(entity =>
            {
                entity.ToTable("CVBuilder_Technology");

                entity.Property(e => e.CvbuilderId).HasColumnName("CVBuilderId");

                entity.HasOne(d => d.Cvbuilder)
                    .WithMany(p => p.CvbuilderTechnology)
                    .HasForeignKey(d => d.CvbuilderId)
                    .HasConstraintName("FK__CVBuilder__CVBui__4E7E8A33");
                entity.HasOne(d => d.Technology)
                   .WithMany(p => p.CvbuilderTechnology)
                   .HasForeignKey(d => d.TechnologyId)
                   .HasConstraintName("FK__CVBuilder__Techn__4F72AE6C");
            });
            modelBuilder.Entity<CvBuilderEstimatePrice>(entity =>
            {
                entity.ToTable("CvBuilder_EstimatePrice");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            });
            modelBuilder.Entity<DocumentLibrary>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentTitle).HasMaxLength(500);

                entity.Property(e => e.DocumentType).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Version).HasMaxLength(50);
            });
        }

        #region Store Procedure Calling

        public async Task<List<GetMonthTimesheets_Result>> GetMonthTimesheets(DateTime startDate, DateTime endDate, string CRMIds)
        {
            // Initialization.  
            List<GetMonthTimesheets_Result> lst = new List<GetMonthTimesheets_Result>();

            try
            {
                // Settings.  
                SqlParameter startDateParam = new SqlParameter("@monthStartDate", startDate);
                SqlParameter endDateParam = new SqlParameter("@monthEndDate", endDate);
                SqlParameter CRMIdsParam = new SqlParameter("@CRMIds", CRMIds);

                // Processing.  
                string sqlQuery = "EXEC [dbo].[GetMonthTimesheets] " +
                                    "@monthStartDate," + "@monthEndDate," + "@CRMIds";

                lst = await this.Query<GetMonthTimesheets_Result>().FromSql(sqlQuery, startDateParam, endDateParam, CRMIdsParam).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Info.  
            return lst;
        }
        public async Task<List<GetMonthTimesheetsNew_Result>> GetMonthTimesheets_New(DateTime startDate, DateTime endDate, string CRMIds)
        {
            // Initialization.  
            List<GetMonthTimesheetsNew_Result> lst = new List<GetMonthTimesheetsNew_Result>();

            try
            {
                // Settings.  
                SqlParameter startDateParam = new SqlParameter("@monthStartDate", startDate);
                SqlParameter endDateParam = new SqlParameter("@monthEndDate", endDate);
                SqlParameter CRMIdsParam = new SqlParameter("@CRMIds", CRMIds);

                // Processing.  
                string sqlQuery = "EXEC [dbo].[GetMonthTimesheets_New] " +
                                    "@monthStartDate," + "@monthEndDate," + "@CRMIds";

                lst = await this.Query<GetMonthTimesheetsNew_Result>().FromSql(sqlQuery, startDateParam, endDateParam, CRMIdsParam).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Info.  
            return lst;
        }
        public async Task<List<GetTimesheetsByTimesheetIds_Result>> GetTimesheetsByTimesheetIds(string TimesheetIds, int PageNumber, int PageSize)
        {
            // Initialization.  
            List<GetTimesheetsByTimesheetIds_Result> lst = new List<GetTimesheetsByTimesheetIds_Result>();

            try
            {
                // Settings.  
                SqlParameter TimesheetIdsparam = new SqlParameter("@TimesheetIds", TimesheetIds);
                SqlParameter PageNumberparam = new SqlParameter("@PageNumber", PageNumber);
                SqlParameter PageSizeParam = new SqlParameter("@PageSize", PageSize);

                // Processing.  
                string sqlQuery = "EXEC [dbo].[GetTimesheetsByTimesheetIds] " +
                                    "@TimesheetIds," + "@PageNumber," + "@PageSize";

                lst = await this.Query<GetTimesheetsByTimesheetIds_Result>().FromSql(sqlQuery, TimesheetIdsparam, PageNumberparam, PageSizeParam).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Info.  
            return lst;
        }
        public async Task<List<GETMOMListByMeetingMasterID_Result>> GETMOMListByMeetingMasterID(int MeetingMasterID)
        {
            // Initialization.  
            List<GETMOMListByMeetingMasterID_Result> lst = new List<GETMOMListByMeetingMasterID_Result>();

            try
            {
                // Settings.  
                SqlParameter MeetingMaster = new SqlParameter("@MeetingMasterID", MeetingMasterID);


                // Processing.  
                string sqlQuery = "EXEC [dbo].[GETMOMListByMeetingMasterID] " +
                             "@MeetingMasterID";


                lst = await this.Query<GETMOMListByMeetingMasterID_Result>().FromSql(sqlQuery, MeetingMaster).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Info.  
            return lst;
        }
        public async Task<List<AllTeamStatusReportGraph_Result>> GetAllTeamActiveUserOnDate(int PmUId, DateTime? fromdate, DateTime? todate)
        {
            // Initialization.  
            List<AllTeamStatusReportGraph_Result> lst = new List<AllTeamStatusReportGraph_Result>();

            try
            {
                // Settings.  
                SqlParameter PMUId = new SqlParameter("@PMUId", PmUId);
                SqlParameter from_date = new SqlParameter("@from_date", fromdate);
                SqlParameter to_date = new SqlParameter("@todate", todate);

                // Processing.  
                string sqlQuery = "EXEC [dbo].[GetAllTeamActiveUserOnDate] " +
                                    "@PMUId," + "@from_date," + "@todate";

                lst = await this.Query<AllTeamStatusReportGraph_Result>().FromSql(sqlQuery, PMUId, from_date, to_date).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Info.  
            return lst;
        }
        public async Task<List<TeamStatusReportGraphDetail_Result>> GetAllAciveUserDetailOnDate(int PmUid, DateTime? selecteddate)
        {
            // Initialization.  
            List<TeamStatusReportGraphDetail_Result> lst = new List<TeamStatusReportGraphDetail_Result>();

            try
            {
                // Settings.  
                SqlParameter PMUId = new SqlParameter("@PMUId", PmUid);
                SqlParameter selected_date = new SqlParameter("@selecteddate", selecteddate);

                // Processing.  
                string sqlQuery = "EXEC [dbo].[GetAllAciveUserDetailOnDate] " +
                                    "@PMUId," + "@selecteddate";

                lst = await this.Query<TeamStatusReportGraphDetail_Result>().FromSql(sqlQuery, PMUId, selected_date).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Info.  
            return lst;
        }
        public async Task<List<TeamStatusReportGraph_Result>> GetAciveUserOnDate(int TlId, DateTime? fromdate, DateTime? todate)
        {
            // Initialization.  
            List<TeamStatusReportGraph_Result> lst = new List<TeamStatusReportGraph_Result>();

            try
            {
                // Settings.  
                SqlParameter TLId = new SqlParameter("@TLId", TlId);
                SqlParameter from_date = new SqlParameter("@from_date", fromdate);
                SqlParameter to_date = new SqlParameter("@todate", todate);

                // Processing.  
                string sqlQuery = "EXEC [dbo].[GetAciveUserOnDate] " +
                                    "@TLId," + "@from_date," + "@todate";

                lst = await this.Query<TeamStatusReportGraph_Result>().FromSql(sqlQuery, TLId, from_date, to_date).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Info.  
            return lst;
        }

        public async Task<List<TeamStatusReportGraphDetail_Result>> GetAciveUserDetailOnDate(int TlId, DateTime? selecteddate)
        {
            // Initialization.  
            List<TeamStatusReportGraphDetail_Result> lst = new List<TeamStatusReportGraphDetail_Result>();

            try
            {
                // Settings.  
                SqlParameter TLId = new SqlParameter("@TLId", TlId);
                SqlParameter selected_date = new SqlParameter("@selecteddate", selecteddate);

                // Processing.  
                string sqlQuery = "EXEC [dbo].[GetAciveUserDetailOnDate] " +
                                    "@TLId," + "@selecteddate";

                lst = await this.Query<TeamStatusReportGraphDetail_Result>().FromSql(sqlQuery, TLId, selected_date).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Info.  
            return lst;
        }


        #endregion
    }
}
