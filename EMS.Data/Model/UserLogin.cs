using EMS.Data.Model;
using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class UserLogin
    {
        public UserLogin()
        {
            AccessoryCreateByU = new HashSet<Accessory>();
            AccessoryModifyByU = new HashSet<Accessory>();
            AlertMessage = new HashSet<AlertMessage>();
            AlertMessageRead = new HashSet<AlertMessageRead>();
            Appraisal = new HashSet<Appraisal>();
            AvailUserU = new HashSet<AvailUser>();
            AvailUserUser = new HashSet<AvailUser>();
            Complaint = new HashSet<Complaint>();
            ComplaintUser = new HashSet<ComplaintUser>();
            //ComplaintAddedByNavigation = new HashSet<Complaint>();
            //ComplaintEmployee = new HashSet<Complaint>();
            Component = new HashSet<Component>();
            ConferenceRoomBooking = new HashSet<ConferenceRoomBooking>();
            DesignerManagementAddedU = new HashSet<DesignerManagement>();
            DesignerManagementAssignU = new HashSet<DesignerManagement>();
            DeviceCreateByU = new HashSet<Device>();
            DeviceDetailAssignedToU = new HashSet<DeviceDetail>();
            DeviceDetailCreateByU = new HashSet<DeviceDetail>();
            DeviceDetailModifyByU = new HashSet<DeviceDetail>();
            DeviceDetailSubmitToU = new HashSet<DeviceDetail>();
            DeviceDeviceHistory = new HashSet<DeviceDeviceHistory>();
            DeviceDeviceInfo = new HashSet<DeviceDeviceInfo>();
            DeviceModifyByU = new HashSet<Device>();
            DevicePmu = new HashSet<Device>();
            ElanceAssignedJob = new HashSet<ElanceAssignedJob>();
            EmpLateActivity = new HashSet<EmpLateActivity>();
            EmployeeAppraiseEmployee = new HashSet<EmployeeAppraise>();
            EmployeeAppraiseUser = new HashSet<EmployeeAppraise>();
            EmployeeComplaintEmployee = new HashSet<EmployeeComplaint>();
            EmployeeComplaintUser = new HashSet<EmployeeComplaint>();
            EmployeeFeedback = new HashSet<EmployeeFeedback>();
            EmployeeMedicalData = new HashSet<EmployeeMedicalData>();
            EscalationConclusion = new HashSet<EscalationConclusion>();
            EsculationForUser = new HashSet<EsculationForUser>();
            EsculationFoundForUser = new HashSet<EsculationFoundForUser>();
            EstimateDocument = new HashSet<EstimateDocument>();
            ExpenseApprovedByU = new HashSet<Expense>();
            ExpenseCreateByU = new HashSet<Expense>();
            ExpenseModifyByU = new HashSet<Expense>();
            ExpenseReimbursedByU = new HashSet<Expense>();
            Forecasting = new HashSet<Forecasting>();
            ForecastingReviewedU = new HashSet<Forecasting>();
            ForumFeedback = new HashSet<ForumFeedback>();
            Forums = new HashSet<Forums>();
            FullLeave = new HashSet<FullLeave>();
            HalfLeave = new HashSet<HalfLeave>();
            Investment = new HashSet<Investment>();
            JobReference = new HashSet<JobReference>();
            KnowledgeBase = new HashSet<KnowledgeBase>();
            LateHourModifiedByU = new HashSet<LateHour>();
            LateHourU = new HashSet<LateHour>();
            LeadTechnician = new HashSet<LeadTechnician>();
            LeadTechnicianArchive = new HashSet<LeadTechnicianArchive>();
            LeadTransaction = new HashSet<LeadTransaction>();
            LeadTransactionArchive = new HashSet<LeadTransactionArchive>();
            LeaveActivities1 = new HashSet<LeaveActivity>();
            LeaveActivities = new HashSet<LeaveActivity>();
            LeaveAdjust = new HashSet<LeaveAdjust>();
            MeetingMaster = new HashSet<MeetingMaster>();
            LeaveActivityModifyByNavigation = new HashSet<LeaveActivity>();
            MomMeetingAuthorByU = new HashSet<MomMeeting>();
            MomMeetingChairedByU = new HashSet<MomMeeting>();
            MomMeetingParticipant = new HashSet<MomMeetingParticipant>();
            MomMeetingTaskParticipant = new HashSet<MomMeetingTaskParticipant>();
            MomMeetingTaskTimeLine = new HashSet<MomMeetingTaskTimeLine>();
            OrgDocumentApprove = new HashSet<OrgDocumentApprove>();
            OrgDocumentCreateByU = new HashSet<OrgDocument>();
            OrgDocumentModifyByU = new HashSet<OrgDocument>();
            PfreviewSubmittedReviewByU = new HashSet<PfreviewSubmitted>();
            PfreviewSubmittedReviewOnU = new HashSet<PfreviewSubmitted>();
            PilogCreateByU = new HashSet<PILog>();
            PilogModifyByU = new HashSet<PILog>();
            Portfolio = new HashSet<Portfolio>();
            ProductLandingCreateByU = new HashSet<ProductLanding>();
            ProductLandingModifyByU = new HashSet<ProductLanding>();
            Project = new HashSet<Project>();
            ProjectAdditionalSupportApproveByU = new HashSet<ProjectAdditionalSupport>();
            ProjectAdditionalSupportRequestByU = new HashSet<ProjectAdditionalSupport>();
            ProjectAdditionalSupportTl = new HashSet<ProjectAdditionalSupport>();
            ProjectAdditionalSupportUser = new HashSet<ProjectAdditionalSupportUser>();
            ProjectClosureAddedByNavigation = new HashSet<ProjectClosure>();
            ProjectClosureDetail = new HashSet<ProjectClosureDetail>();
            ProjectClosurePm = new HashSet<ProjectClosure>();
            ProjectClosureReviewCreateByU = new HashSet<ProjectClosureReview>();
            ProjectClosureReviewModifyByU = new HashSet<ProjectClosureReview>();
            ProjectClosureUidBaNavigation = new HashSet<ProjectClosure>();
            ProjectClosureUidDevNavigation = new HashSet<ProjectClosure>();
            ProjectClosureUidTlNavigation = new HashSet<ProjectClosure>();
            ProjectDevelopers = new HashSet<ProjectDeveloper>();
            ProjectDeveloperAddon = new HashSet<ProjectDeveloperAddon>();
            ProjectInvoiceUidBaNavigation = new HashSet<ProjectInvoice>();
            ProjectInvoiceUidTlNavigation = new HashSet<ProjectInvoice>();
            ProjectLeadArchiveCommunicator = new HashSet<ProjectLeadArchive>();
            ProjectLeadArchiveOwner = new HashSet<ProjectLeadArchive>();
            ProjectLeadCommunicator = new HashSet<ProjectLead>();
            ProjectLeadOwner = new HashSet<ProjectLead>();
            ProjectLessonCreateByU = new HashSet<ProjectLesson>();
            ProjectLessonModifyByU = new HashSet<ProjectLesson>();
            ProjectNclogAuditeeU = new HashSet<ProjectNCLog>();
            ProjectNclogAuditorU = new HashSet<ProjectNCLog>();
            ProjectPm = new HashSet<ProjectPm>();
            ReadMessage = new HashSet<ReadMessage>();
            ReportBug = new HashSet<ReportBug>();
            SaturdayManagement = new HashSet<SaturdayManagement>();
            SimCreateByU = new HashSet<Sim>();
            SimModifyByU = new HashSet<Sim>();
            Task = new HashSet<Task>();
            TaskAssignedTo = new HashSet<TaskAssignedTo>();
            TaskComment = new HashSet<TaskComment>();
            UserActivities = new HashSet<UserActivity>();
            UserActivityLog = new HashSet<UserActivityLog>();
            UserLog = new HashSet<UserLog>();
            User_Tech = new HashSet<User_Tech>();
            UserTimeSheet = new HashSet<UserTimeSheet>();
            UserTimeSheets1 = new HashSet<UserTimeSheet>();
            ProjectOtherPm = new HashSet<ProjectOtherPm>();

            DomainExperts = new HashSet<DomainExperts>();
            LibraryAddedByNavigation = new HashSet<Library>();
            LibraryModifyByU = new HashSet<Library>();
            LibraryDownloadHistory = new HashSet<LibraryDownloadHistory>();
            LibraryDownloadPermissionAllowedDownloadByNavigation = new HashSet<LibraryDownloadPermission>();
            LibraryDownloadPermissionUserLogin = new HashSet<LibraryDownloadPermission>();
            UserNoc = new HashSet<UserNoc>();

            UserExitProcess = new HashSet<UserExitProcess>();
            ProjectClientFeedback = new HashSet<ProjectClientFeedback>();
            OrgImprovementAddedByU = new HashSet<OrgImprovement>();
            OrgImprovementEmployeeU = new HashSet<OrgImprovement>();
            LibraryAuthorU = new HashSet<Library>();

            AbroadPM = new HashSet<AbroadPM>();
            LessonLearnt = new HashSet<LessonLearnt>();

            EstimateHourBau = new HashSet<EstimateHour>();
            EstimateHourTlu = new HashSet<EstimateHour>();
            EstimatePriceCalculation = new HashSet<EstimatePriceCalculation>();
            Escalation = new HashSet<Escalation>();
            UserDocument = new HashSet<UserDocument>();
            VaccinationStatus = new HashSet<VaccinationStatus>();
            TeamHierarchy = new HashSet<TeamHierarchy>();

            //Wfhactivity = new HashSet<Wfhactivity>();

            InversePmu = new HashSet<UserLogin>();
            WfhactivityApprovedBy = new HashSet<Wfhactivity>();
            WfhactivityU = new HashSet<Wfhactivity>();

            TdsempDeductionCreatedByNavigation = new HashSet<TdsempDeduction>();
            TdsempDeductionModifyByAcNavigation = new HashSet<TdsempDeduction>();
            TdsempDeductionModifyByEmpNavigation = new HashSet<TdsempDeduction>();
            TdsempDeductionU = new HashSet<TdsempDeduction>();
            Cvbuilder = new HashSet<Cvbuilder>();

            StudyDocumentsAddedbyNavigation = new HashSet<StudyDocuments>();
            StudyDocumentsPermissions = new HashSet<StudyDocumentsPermissions>();
            StudyDocumentsUpdatedbyNavigation = new HashSet<StudyDocuments>();
            RequestedStudyDocuments = new HashSet<RequestedStudyDocuments>();
        }

        public int Uid { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public int? DeptId { get; set; }
        public int? RoleId { get; set; }
        public int? TLId { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? JoinedDate { get; set; }
        public DateTime? AddDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string IP { get; set; }
        public string EmailOffice { get; set; }
        public string EmailPersonal { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternativeNumber { get; set; }
        public string Address { get; set; }
        public string PasswordBackUp { get; set; }
        public string SkypeId { get; set; }
        public DateTime? MarraigeDate { get; set; }
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public int? PMUid { get; set; }
        public bool? IsSuperAdmin { get; set; }
        public int? CRMUserId { get; set; }
        public string ApiPassword { get; set; }
        public string PanNumber { get; set; }
        public string AadharNumber { get; set; }
        public string PassportNumber { get; set; }
        public int? BloodGroupId { get; set; }
        public int? HRMId { get; set; }
        public int? AttendenceId { get; set; }
        public string EmpCode { get; set; }
        public bool IsResigned { get; set; }
        public bool IsSPEG { get; set; }
        public DateTime? ResignationDate { get; set; }
        public DateTime? RelievingDate { get; set; }
        public bool? IsInterestedPffaccount { get; set; }
        public bool? IsFromDbdt { get; set; }
        public bool? IsExpensesAllowed { get; set; }
        public string UANNumber { get; set; }
        public virtual BloodGroup BloodGroup { get; set; }
        public virtual Department Department { get; set; }
        public virtual Role Role { get; set; }
        public virtual Designation Designation { get; set; }
        public string PasswordKey { get; set; }

        public int? DesignationId { get; set; }
        public bool? isAbscond { get; set; }
        public bool? Terminated { get; set; }
        public virtual ICollection<Accessory> AccessoryCreateByU { get; set; }
        public virtual ICollection<Accessory> AccessoryModifyByU { get; set; }
        public virtual ICollection<AlertMessage> AlertMessage { get; set; }
        public virtual ICollection<AlertMessageRead> AlertMessageRead { get; set; }
        public virtual ICollection<Appraisal> Appraisal { get; set; }
        public virtual ICollection<AvailUser> AvailUserU { get; set; }
        public virtual ICollection<AvailUser> AvailUserUser { get; set; }
        //public virtual ICollection<Complaint> ComplaintAddedByNavigation { get; set; }
        //public virtual ICollection<Complaint> ComplaintEmployee { get; set; }
        public virtual ICollection<Complaint> Complaint { get; set; }
        public virtual ICollection<ComplaintUser> ComplaintUser { get; set; }
        public virtual ICollection<Component> Component { get; set; }
        public virtual ICollection<ConferenceRoomBooking> ConferenceRoomBooking { get; set; }
        public virtual ICollection<DesignerManagement> DesignerManagementAddedU { get; set; }
        public virtual ICollection<DesignerManagement> DesignerManagementAssignU { get; set; }
        public virtual ICollection<Device> DeviceCreateByU { get; set; }
        public virtual ICollection<DeviceDetail> DeviceDetailAssignedToU { get; set; }
        public virtual ICollection<DeviceDetail> DeviceDetailCreateByU { get; set; }
        public virtual ICollection<DeviceDetail> DeviceDetailModifyByU { get; set; }
        public virtual ICollection<DeviceDetail> DeviceDetailSubmitToU { get; set; }
        public virtual ICollection<DeviceDeviceHistory> DeviceDeviceHistory { get; set; }
        public virtual ICollection<DeviceDeviceInfo> DeviceDeviceInfo { get; set; }
        public virtual ICollection<Device> DeviceModifyByU { get; set; }
        public virtual ICollection<Device> DevicePmu { get; set; }
        public virtual ICollection<ElanceAssignedJob> ElanceAssignedJob { get; set; }
        public virtual ICollection<EmpLateActivity> EmpLateActivity { get; set; }
        public virtual ICollection<EmployeeAppraise> EmployeeAppraiseEmployee { get; set; }
        public virtual ICollection<EmployeeAppraise> EmployeeAppraiseUser { get; set; }
        public virtual ICollection<EmployeeComplaint> EmployeeComplaintEmployee { get; set; }
        public virtual ICollection<EmployeeComplaint> EmployeeComplaintUser { get; set; }
        public virtual ICollection<EmployeeFeedback> EmployeeFeedback { get; set; }
        public virtual ICollection<EmployeeMedicalData> EmployeeMedicalData { get; set; }
        public virtual ICollection<EscalationConclusion> EscalationConclusion { get; set; }
        public virtual ICollection<EsculationForUser> EsculationForUser { get; set; }
        public virtual ICollection<EsculationFoundForUser> EsculationFoundForUser { get; set; }
        public virtual ICollection<EstimateDocument> EstimateDocument { get; set; }
        public virtual ICollection<Expense> ExpenseApprovedByU { get; set; }
        public virtual ICollection<Expense> ExpenseCreateByU { get; set; }
        public virtual ICollection<Expense> ExpenseModifyByU { get; set; }
        public virtual ICollection<Expense> ExpenseReimbursedByU { get; set; }
        public virtual ICollection<Forecasting> Forecasting { get; set; }
        public virtual ICollection<Forecasting> ForecastingReviewedU { get; set; }
        public virtual ICollection<ForumFeedback> ForumFeedback { get; set; }
        public virtual ICollection<Forums> Forums { get; set; }
        public virtual ICollection<FullLeave> FullLeave { get; set; }
        public virtual ICollection<HalfLeave> HalfLeave { get; set; }
        public virtual ICollection<Investment> Investment { get; set; }
        public virtual ICollection<JobReference> JobReference { get; set; }
        public virtual ICollection<KnowledgeBase> KnowledgeBase { get; set; }
        public virtual ICollection<LateHour> LateHourModifiedByU { get; set; }
        public virtual ICollection<LateHour> LateHourU { get; set; }
        public virtual ICollection<LeadTechnician> LeadTechnician { get; set; }
        public virtual ICollection<LeadTechnicianArchive> LeadTechnicianArchive { get; set; }
        public virtual ICollection<LeadTransaction> LeadTransaction { get; set; }
        public virtual ICollection<LeadTransactionArchive> LeadTransactionArchive { get; set; }
        public virtual ICollection<LeaveActivity> LeaveActivities1 { get; set; }
        public virtual ICollection<LeaveActivity> LeaveActivities { get; set; }
        public virtual ICollection<LeaveAdjust> LeaveAdjust { get; set; }
        public virtual ICollection<MeetingMaster> MeetingMaster { get; set; }

        public virtual ICollection<MomMeeting> MomMeetingAuthorByU { get; set; }
        public virtual ICollection<MomMeeting> MomMeetingChairedByU { get; set; }
        public virtual ICollection<MomMeetingParticipant> MomMeetingParticipant { get; set; }
        public virtual ICollection<MomMeetingTaskParticipant> MomMeetingTaskParticipant { get; set; }
        public virtual ICollection<MomMeetingTaskTimeLine> MomMeetingTaskTimeLine { get; set; }
        public virtual ICollection<OrgDocumentApprove> OrgDocumentApprove { get; set; }
        public virtual ICollection<OrgDocument> OrgDocumentCreateByU { get; set; }
        public virtual ICollection<OrgDocument> OrgDocumentModifyByU { get; set; }
        public virtual ICollection<PfreviewSubmitted> PfreviewSubmittedReviewByU { get; set; }
        public virtual ICollection<PfreviewSubmitted> PfreviewSubmittedReviewOnU { get; set; }
        public virtual ICollection<PILog> PilogCreateByU { get; set; }
        public virtual ICollection<PILog> PilogModifyByU { get; set; }
        public virtual ICollection<Portfolio> Portfolio { get; set; }
        public virtual ICollection<ProductLanding> ProductLandingCreateByU { get; set; }
        public virtual ICollection<ProductLanding> ProductLandingModifyByU { get; set; }
        public virtual ICollection<Project> Project { get; set; }
        public virtual ICollection<ProjectAdditionalSupport> ProjectAdditionalSupportApproveByU { get; set; }
        public virtual ICollection<ProjectAdditionalSupport> ProjectAdditionalSupportRequestByU { get; set; }
        public virtual ICollection<ProjectAdditionalSupport> ProjectAdditionalSupportTl { get; set; }

        //public virtual ICollection<ProjectAdditionalSupport> ProjectAdditionalSupports21 { get; set; }
        public virtual ICollection<LeaveActivity> LeaveActivityModifyByNavigation { get; set; }
        public virtual ICollection<ProjectAdditionalSupportUser> ProjectAdditionalSupportUser { get; set; }
        public virtual ICollection<ProjectClosure> ProjectClosureAddedByNavigation { get; set; }
        public virtual ICollection<ProjectClosureDetail> ProjectClosureDetail { get; set; }
        public virtual ICollection<ProjectClosure> ProjectClosurePm { get; set; }
        public virtual ICollection<ProjectClosureReview> ProjectClosureReviewCreateByU { get; set; }
        public virtual ICollection<ProjectClosureReview> ProjectClosureReviewModifyByU { get; set; }
        public virtual ICollection<ProjectClosure> ProjectClosureUidBaNavigation { get; set; }
        public virtual ICollection<ProjectClosure> ProjectClosureUidDevNavigation { get; set; }
        public virtual ICollection<ProjectClosure> ProjectClosureUidTlNavigation { get; set; }
        public virtual ICollection<ProjectDeveloper> ProjectDevelopers { get; set; }
        public virtual ICollection<ProjectDeveloperAddon> ProjectDeveloperAddon { get; set; }
        public virtual ICollection<ProjectInvoice> ProjectInvoiceUidBaNavigation { get; set; }
        public virtual ICollection<ProjectInvoice> ProjectInvoiceUidTlNavigation { get; set; }
        public virtual ICollection<ProjectLeadArchive> ProjectLeadArchiveCommunicator { get; set; }
        public virtual ICollection<ProjectLeadArchive> ProjectLeadArchiveOwner { get; set; }
        public virtual ICollection<ProjectLead> ProjectLeadCommunicator { get; set; }
        public virtual ICollection<ProjectLead> ProjectLeadOwner { get; set; }
        public virtual ICollection<ProjectLesson> ProjectLessonCreateByU { get; set; }
        public virtual ICollection<ProjectLesson> ProjectLessonModifyByU { get; set; }
        public virtual ICollection<ProjectNCLog> ProjectNclogAuditeeU { get; set; }
        public virtual ICollection<ProjectNCLog> ProjectNclogAuditorU { get; set; }
        public virtual ICollection<ProjectPm> ProjectPm { get; set; }
        public virtual ICollection<ReadMessage> ReadMessage { get; set; }
        public virtual ICollection<ReportBug> ReportBug { get; set; }
        public virtual ICollection<SaturdayManagement> SaturdayManagement { get; set; }
        public virtual ICollection<Sim> SimCreateByU { get; set; }
        public virtual ICollection<Sim> SimModifyByU { get; set; }
        public virtual ICollection<Task> Task { get; set; }
        public virtual ICollection<TaskAssignedTo> TaskAssignedTo { get; set; }
        public virtual ICollection<TaskComment> TaskComment { get; set; }
        public virtual ICollection<UserActivity> UserActivities { get; set; }
        public virtual ICollection<UserActivityLog> UserActivityLog { get; set; }
        public virtual ICollection<UserLog> UserLog { get; set; }
        public virtual ICollection<User_Tech> User_Tech { get; set; }
        public virtual ICollection<UserTimeSheet> UserTimeSheet { get; set; }
        public virtual ICollection<UserTimeSheet> UserTimeSheets1 { get; set; }
        public string OtherTechnology { get; set; }
        public virtual ICollection<DomainExperts> DomainExperts { get; set; }
        public virtual ICollection<ProjectOtherPm> ProjectOtherPm { get; set; }
        public virtual ICollection<UserNoc> UserNoc { get; set; }

        public virtual ICollection<UserExitProcess> UserExitProcess { get; set; }
        public virtual ICollection<Library> LibraryAddedByNavigation { get; set; }
        public virtual ICollection<Library> LibraryModifyByU { get; set; }
        public virtual ICollection<LibraryDownloadHistory> LibraryDownloadHistory { get; set; }
        public virtual ICollection<LibraryDownloadPermission> LibraryDownloadPermissionAllowedDownloadByNavigation { get; set; }
        public virtual ICollection<LibraryDownloadPermission> LibraryDownloadPermissionUserLogin { get; set; }
        public virtual ICollection<ProjectClientFeedback> ProjectClientFeedback { get; set; }

        public virtual ICollection<OrgImprovement> OrgImprovementAddedByU { get; set; }
        public virtual ICollection<OrgImprovement> OrgImprovementEmployeeU { get; set; }
        public virtual ICollection<Library> LibraryAuthorU { get; set; }
        public virtual ICollection<Library> LibraryUidBaNavigation { get; set; }
        public virtual ICollection<Library> LibraryUidTlNavigation { get; set; }
        public virtual ICollection<SelfDeclaration> SelfDeclaration { get; set; }
        public virtual ICollection<AbroadPM> AbroadPM { get; set; }
        public virtual ICollection<LessonLearnt> LessonLearnt { get; set; }

        public virtual ICollection<EstimateHour> EstimateHourBau { get; set; }
        public virtual ICollection<EstimateHour> EstimateHourTlu { get; set; }
        public virtual ICollection<EstimatePriceCalculation> EstimatePriceCalculation { get; set; }
        public virtual ICollection<EstimatePriceCalculation> EstimatePriceCalculationModifiedByU { get; set; }
        public virtual ICollection<Escalation> Escalation { get; set; }
        public virtual ICollection<UserDocument> UserDocument { get; set; }
        public virtual UserLogin Pmu { get; set; }
        public virtual ICollection<UserLogin> InversePmu { get; set; }
        public virtual ICollection<VaccinationStatus> VaccinationStatus { get; set; }
        public virtual ICollection<TeamHierarchy> TeamHierarchy { get; set; }

        //public virtual ICollection<Wfhactivity> Wfhactivity { get; set; }

        //  public virtual UserLogin Pmu { get; set; }
        // public virtual ICollection<UserLogin> InversePmu { get; set; }
        public virtual ICollection<Wfhactivity> WfhactivityApprovedBy { get; set; }
        public virtual ICollection<Wfhactivity> WfhactivityU { get; set; }
        public virtual ICollection<TdsempDeduction> TdsempDeductionCreatedByNavigation { get; set; }
        public virtual ICollection<TdsempDeduction> TdsempDeductionModifyByAcNavigation { get; set; }
        public virtual ICollection<TdsempDeduction> TdsempDeductionModifyByEmpNavigation { get; set; }
        public virtual ICollection<TdsempDeduction> TdsempDeductionU { get; set; }

        public virtual ICollection<StudyDocuments> StudyDocumentsAddedbyNavigation { get; set; }
        public virtual ICollection<StudyDocumentsPermissions> StudyDocumentsPermissions { get; set; }
        public virtual ICollection<StudyDocuments> StudyDocumentsUpdatedbyNavigation { get; set; }
        public virtual ICollection<RequestedStudyDocuments> RequestedStudyDocuments { get; set; }

        public virtual ICollection<Cvbuilder> Cvbuilder { get; set; }
    }
}
