using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core
{
    public static class Enums
    {
        public enum AppraisalMarking
        {
            Productivity = 1,
            Communication = 2,
            LeaderShip = 3,
            PersonalDevelopment = 4,
            RelationShips = 5,
            Management = 6
        }
        public enum EstimateDocuments
        {
            ProposalDocument = 1,
            WireframeMockups = 2,
            OtherDocument = 3,
            DSPhotos = 4,
            Flowchart = 5
        }
        public enum UserRoles
        {
            //DV = 1,
            //SD = 2,
            //TL = 3,
            PM = 4,
            // HR = 5,
            //QA = 6,
            PMO = 7,
            // DG = 8,
            // BA = 9,
            //TLS = 10,
            //SEO = 11,
            //OP = 12,
            DEO = 13,
            UKPM = 14,
            //NW = 15,
            AC = 16,
            ST = 17,
            OTH = 18,
            UKBDM = 19,
            Director = 20,
            PMOAU = 21,
            AUPM = 22,
            JD = 23,
            //CW=24,
            //
            DV = 25, //1
            DVPManagerial = 26,
            DVManagerial = 27,
            QA = 28, //6
            QAPManagerial = 29,
            QAManagerial = 30,
            BAPreSales = 31,
            BAPrePostSales = 32,
            AccountsAdminCategory = 33,
            HROperations = 34, //12
            HRPayroll = 35, //HROperationsPayrollTA 35
            HRBP = 36,
            DMCManagingContent = 37, //24
            DMCContentWriter = 38,
            DMCSEO = 39, // 11
            DMCPPC = 40,
            DMCProductMarketingManager = 41,
            DMCPaidAdvertisingManager = 42,
            DMCDigitalAdvertisingManager = 43,
            DMCPaidSearchManager = 44,
            DMCDigitalMarketer = 45,
            UIUXDesigner = 46,
            UIUXFrontEndDeveloper = 47,
            UIUXMeanStackDeveloper = 48,
            UIUXDeveloper = 49,
            UIUXManagerial = 50,
            NWLANWAN = 51,
            NWProblemResolution = 52,
            NWHardwareSoftware = 53,
            NWLeadRole = 54, //15
            NWCloudLeadRole = 55,
            DMCSMO = 56,
            GamingDesignerDeveloper = 57,
            GamingDesigner = 58,
            GamingDeveloper = 59,
            GamingManagerial = 60,
            HRTA = 61, //12
            HRComp = 62,
            HRTD = 63,
            HRTAOps = 64,
            HRTAComp = 65,
            BAPostSales = 66,
            Trainee = 67
        }
        // # Added by Pooja #
        public enum ModalSize
        {
            Small,
            Large,
            Medium,
            XLarge
        }
        public enum ActivityStatus
        {
            Away = 1,
            Working = 2,
            Free = 3,
            NotLoggedIn = 4,
            Leave = 5
        }

        public enum ActivityStatusOrder : byte
        {
            Free = 1,
            AdditionalSupport = 2,
            WorkingOverrun = 3,
            SupportTeam = 4,
            Running = 5,
            InHouse = 6,
            NotLoggedIn = 7,
            NotLoggedInLeavePending = 8,
            NotLoggedInLeaveUnApproved = 9,
            LeaveHalf = 10,
            Leave = 11,
            Other = 100
        }

        public enum LeaveStatus
        {
            Pending = 6,
            Approved = 7,
            Cancelled = 8,
            UnApproved = 9
        }

        public enum WFHStatus
        {
            Pending = 6,
            Approved = 7,
            Cancelled = 8,
            UnApproved = 9
        }

        public enum LeaveType
        {
            Normal = 15,
            Urgent = 16,
            Adjust = 17,
            Auto = 18
        }

        public enum WFHCategory
        {
            [Description(""), Display(Name = "")]
            NA = 0,
            [Description("Full Day"), Display(Name = "Full Day", ShortName = "F")]
            Full = 1,
            [Description("Half Day"), Display(Name = "Half Day", ShortName = "H")]
            Half = 2
        }

        public enum LeaveCategory
        {
            [Description(""), Display(Name = "")]
            NA = 0,
            [Description("Loss of pay leave"), Display(Name = "Loss Of Pay", ShortName = "A")]
            UnpaidLeave = 1,
            [Description("Compensatory off"), Display(Name = "Compensatory Off", ShortName = "CO")]
            CompensatoryOff = 2,
            [Description("Compensatory Work"), Display(Name = "Compensatory Work", ShortName = "CW")]
            CompensatoryWork = 3,
            [Description("On Official Duty"), Display(Name = "On Official Duty", ShortName = "OOD")]
            OnOfficialDuty = 4,
            [Description("Casual leave"), Display(Name = "CL", ShortName = "CL")]
            CasualLeave = 5,
            [Description("Week off"), Display(Name = "Week off", ShortName = "WO")]
            WeekOff = 6,
            [Description("Holiday"), Display(Name = "Holiday", ShortName = "H")]
            Holiday = 7,
            [Description("Paternity leave"), Display(Name = "Paternity Leave", ShortName = "PL")]
            PaternityLeave = 8,
            [Description("Earned leave"), Display(Name = "Earned Leave", ShortName = "EL")]
            EarnedLeave = 9,
            [Description("Sick leave"), Display(Name = "Sick Leave", ShortName = "SL")]
            SickLeave = 10,
            [Description("Maternity leave"), Display(Name = "Maternity Leave", ShortName = "ML")]
            MaternityLeave = 11,
            [Description("Bereavement leave"), Display(Name = "Bereavement Leave", ShortName = "BL")]
            BereavementLeave = 12,
            [Description("Wedding Leave"), Display(Name = "Wedding Leave", ShortName = "WL")]
            WeddingLeave = 13,
            [Description("Loyalty leave"), Display(Name = "Loyalty Leave", ShortName = "LL")]
            LoyaltyLeave = 14

        }

        public enum HolidayType
        {
            [DisplayName("Sick Leave")]
            Sick = 1,
            [DisplayName("Holiday Leave")]
            Holiday = 2
        }

        public enum ProjectDevWorkStatus
        {
            Closed = 11,
            Running = 12
        }
        public enum ProjectDevWorkRole
        {
            Paid = 13,
            Addon = 14
        }

        public enum AddSupportRequestStatus : byte
        {
            Partial = 0,
            Pending = 1,
            Approved = 2,
            UnApproved = 3
        }

        public enum IntwQuestype
        {
            MultipleChoice = 1,
            SingleChoice = 2
        }

        public enum OwnersTypes
        {
            Estimate = 1,
            Communication = 2,
            Technical = 3,
            Technology = 4
        }

        public enum ChasingStatus
        {
            // Chase = 1,
            // Chased = 2,            
            // AlmostConverted=3,
            Pending = 1,
            Converted = 2,
        }

        public enum ChasingType
        {
            IsExistingLead = 1,
            IsExistingClient = 2,
            IsNewClient = 3
        }

        public enum TaskStatus
        {
            Pending = 1,
            Assigned = 2,
            Completed = 3
        }
        public enum MessageType
        {
            Warning,
            Success,
            Danger,
            Info
        }
        public enum Priority
        {
            [Display(Name = "Very High")]
            VeryHigh = 1,
            High = 2,
            Medium = 3,
            Low = 4
        }

        // End
        public static bool CompareDate(this DateTime? dateTime, DateTime? otherDate)
        {
            if (dateTime.HasValue && otherDate.HasValue)
            {
                return dateTime.Value.Day == otherDate.Value.Day
                       && dateTime.Value.Month == otherDate.Value.Month
                       && dateTime.Value.Year == otherDate.Value.Year;
            }

            return false;

        }

        // Enums for bucket model
        public enum BucketModel
        {
            BucketSystemDeveloperOrDesigner = 4,
            ServerNetworkEngineerSupport = 9,
            WebHosting = 11,
            SEO = 8,
            PayPerClick = 19,
            SocialMediaMarketing = 20,
            AppMarketing = 21

        }

        //public enum UserTimeSheet
        //{
        //    //Local

        //    // AdditionalFree=18,



        //    //Live

        //    AdditionalFree = 15,
        //    FreeBugFixing = 35,
        //    Other = 145

        //}
        public enum ProjectGroup
        {

            //local
            //DotNetDevloper = 3,
            //PHPDeveloper = 5,
            //MobileDevloper = 6,
            //QualityAnalyst = 7,
            //Designer = 8,
            //BusinessAnalyst=9
            // ProjectManager=9,

            //Live
            DotNetDevloper = 3,
            PHPDeveloper = 5,
            MobileDevloper = 6,
            QualityAnalyst = 7,
            Designer = 8,
            ProjectManager = 9,
            BusinessAnalyst = 10

        }

        public enum KnowledgeType
        {
            NewConcept = 1,
            ExistingComponent = 2
        }

        //Project Closer
        public enum CloserType
        {
            Pending = 1,
            [Display(Name = "Dead Response")]
            DeadResponse = 3,
            Converted = 4
        }

        public enum ClosureApiRequestStatusType
        {
            Approved = 1,
            Declined = 2
        }

        public enum CRMStatus
        {
            Completed = 1,
            [Display(Name = "On Hold")]
            OnHold = 2,
            Running = 3,
            Remove = 4,
            [Display(Name = "Not Initiated")]
            NotInitiated = 5,
            [Display(Name = "Not Converted")]
            NotConverted = 7,
            [Display(Name = "Over Run")]
            OverRun = 8,
            [Display(Name = "Pending Payment/Completed")]
            PendingPaymentCompleted = 9
        }

        public enum APICRMStatus
        {
            Running = 1,
            [Display(Name = "On Hold")]
            OnHold = 2,
            [Display(Name = "Completed")]
            Complete = 3,
            Remove = 4,
            [Display(Name = "Not Initiated")]
            NotInitiated = 5,
            [Display(Name = "Not Converted")]
            NotConverted = 7,
            [Display(Name = "Over Run")]
            OverRun = 8,
            [Display(Name = "Pending Payment/Completed")]
            PendingPaymentCompleted = 9
        }

        public enum ProjectInvoiceStatus
        {
            [Display(Name = "Pending")]
            Pending = 1,
            [Display(Name = "Partial Payment")]
            PartialPayment = 2,
            [Display(Name = "Paid")]
            Paid = 3,
            Cancelled = 4,
            [Display(Name = "Waiting Approval")]
            WaitingApproval = 5,
            [Display(Name = "Bad Debt")]
            BadDebt = 6
        }

        public enum ClientQualtiy
        {
            Average = 1,
            Promising = 2,
            Poor = 3
        }

        public enum ClientBadge
        {
            PLATINUM = 1,
            GOLD = 2,
            SILVER = 3,
            BRONZE = 4
        }

        public enum ReviewRoleType
        {
            [Display(Name = "Developer")]
            Developer = 1,
            [Display(Name = "Team Leader")]
            TeamLeader = 2,
        }
        public enum ReviewSkillType
        {
            [Display(Name = "Hard Skill")]
            HardSkill = 1,
            [Display(Name = "Soft Skill")]
            SoftSkill = 2,
        }


        public enum PerformanceReviewOptions
        {
            Unexpected = 1,
            [Display(Name = "Meet Expectations")]
            Meet_Expectations = 2,
            [Display(Name = "Exceed Expectations")]
            Exceed_Expectations = 3,
            Outstanding = 4
        }

        public enum BloodGroups
        {
            [Display(Name = "O+")]
            Opos = 1,
            [Display(Name = "O-")]
            Oneg = 2,
            [Display(Name = "A+")]
            Apos = 3,
            [Display(Name = "A-")]
            Aneg = 4,
            [Display(Name = "B+")]
            Bpos = 5,
            [Display(Name = "B-")]
            Bneg = 6,
            [Display(Name = "AB+")]
            ABpos = 7,
            [Display(Name = "AB-")]
            ABneg = 8
        }
        public enum InterViewStaus
        {
            Pending = 0,
            Selected = 1,
            Rejected = 2,
            OnHold = 3
        }
        public enum ProjectStatus
        {

            Hold = 'H',
            Deactive = 'D',
            [Display(Name = "Not Initiated")]
            NotInitiated = 'I',
            Running = 'R',
            Complete = 'C',
            OverRun = 'O',
            [Display(Name = "Not Converted")]
            NotConverted = 'N'
        }

        public enum Month : byte
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }
        public enum Country : byte
        {
            All = 0,
            India = 1,
            UK = 2,
            US = 3,
            AUS = 4,
            UAE = 5
        }
        public enum ProjectDepartment
        {
            [Display(Name = "Quality Analyst")]
            QualityAnalyst = 1,

            [Display(Name = ".Net Development")]
            DotNetDevelopment = 2,

            [Display(Name = "Other")]
            Other = 3,

            [Display(Name = "SEO")]
            SEO = 4,

            [Display(Name = "Mobile Department")]
            MobileApplication = 5,

            [Display(Name = "PHP Development")]
            PHPDevelopment = 6,

            [Display(Name = "JAVA Development")]
            JAVADevelopment = 7,

            [Display(Name = "Web Designing")]
            WebDesigning = 8,

            [Display(Name = "HR Department")]
            HRDepartment = 9,

            [Display(Name = "Account Department")]
            AccountDepartment = 10,

            [Display(Name = "Network/Hardware Department")]
            NetworkHardwareDepartment = 11,

            [Display(Name = "Games Development")]
            GamesDevelopment = 12,

            [Display(Name = "Head Office")]
            HeadOffice = 13,

            [Display(Name = "Business Analyst")]
            BusinessAnalyst = 14,

            [Display(Name = "Salesforce Development")]
            SalesforceDevelopment = 15,

            [Display(Name = "Software Trainee")]
            SoftwareTrainee = 0,

            [Display(Name = "Hub Spot")]
            HubSpot = 20,
        }
        public enum Title
        {
            Mr = 1,
            Miss = 2,
            Mrs = 3,
            Mast = 4
        }

        public enum Gender
        {
            Male = 1,
            Female = 2
        }

        public enum RelationType
        {
            Wife = 1,
            Daughter = 2,
            Son = 3,
            Husband = 4
        }

        public enum TypeValues
        {
            Html = 1,
            Design = 2,
            JS = 3
        }

        public enum FeedbackRankStatus
        {
            [Display(Name = "Strongly Disagree")]
            StronglyDisagree = 1,
            [Display(Name = "Disagree")]
            Disagree = 2,
            [Display(Name = "Agree")]
            Agree = 3,
            [Display(Name = "Strongly Agree")]
            StronglyAgree = 4
        }

        public enum ClientCountry
        {
            UK = 1,
            US,
            AUS,
            India,
            FR,
            UAE
        }

        public enum LeadStatus
        {
            [Display(Name = "Strongly Disagree")]
            Converted = 1,

            [Display(Name = "Escalated")]
            Escalated = 2,

            [Display(Name = "Wrong Understanding")]
            WrongUnderstanding = 3,

            [Display(Name = "Over Estimate")]
            OverEstimate = 4,

            [Display(Name = "Action Required From (Out of India PM)")]
            ActionRequiredFrom_OutOfIndiaPM = 5,

            [Display(Name = "Action Required From (Team)")]
            ActionRequiredFrom_Team = 6,

            [Display(Name = "Chase Request")]
            ChaseRequest = 7,

            [Display(Name = "Leak from company")]
            LeakFromCompany = 8,

            [Display(Name = "Already Converted (From Outside)")]
            AlreadyConvertedFromOutside = 10,

            [Display(Name = "Do Not Chase")]
            DoNotChase = 21,

        }

        public enum TechnologySpecializationType : byte
        {
            Expert = 1,
            Intermediate = 2,
            Beginner = 3,
            Interested = 4
        }        
        public enum TaskStatusType
        {
            Pending = 1,
            Process = 2,
            Completed = 3,
            Closed = 4
        }

        public enum OrgDocumentType : byte
        {
            [Display(Name = "Coding Guidelines")]
            Guidelines = 1,
            [Display(Name = "Code Review Checklist")]
            ReviewChecklist = 2,
            [Display(Name = "Template Document")]
            Template = 3,
            [Display(Name = "Process Document")]
            ProcessDocument = 4,
            [Display(Name = "Other Document")]
            OtherDocument = 5
        }

        public enum InvestmentClaimType : byte
        {
            Exemption = 1,
            Income = 2
        }

        public enum ProjectLessonTopicGroup : byte
        {
            [Display(Name = "Project Management")]
            ProjectManagement = 1,

            [Display(Name = "Technical Management")]
            TechnicalManagement = 2,

            [Display(Name = "Human Factors")]
            HumanFactors = 3,

            Code = 4,
            Other = 5
        }

        public enum ProjectAuditCycle : byte
        {
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4,
            Fifth = 5,
            Sixth = 6,
            Seventh = 7,
            Eight = 8,
            Ninth = 9,
            Tenth = 10,
            Eleventh = 11,
            Twelfth = 12,
            Thirteenth = 13,
            Fourteenth = 14,
            Fifteenth = 15,
            Sixteenth = 16,
            Seventeenth = 17,
            Eigthteenth = 18,
            Ninteenth = 19,
            Twenteenth = 20
        }

        public enum ProjectAuditStatus : byte
        {
            Open = 1,
            Completed = 2,
            Closed = 3
        }

        public enum ProjectAuditType : byte
        {
            Minor = 1,
            Major = 2,
            Observation = 3
        }

        public enum PILogStatus : byte
        {
            Pending = 1,
            InProcess = 2,
            Approved = 3,
            RollOut = 4,
            Cancelled = 10
        }

        public enum DeviceStatus
        {
            Assigned = 1,
            Pending = 2,
            Submitted = 3
        }

        public enum ExpensePaymentThrough : byte
        {
            [Display(Name = "Company Card")]
            CompanyCard = 1,
            [Display(Name = "Cash Or Personal Card")]
            CashOrPersonalCard = 2
        }

        public enum ExpensePaymentStatus : byte
        {
            Pending = 1,
            Approved = 2,
            Rejected = 3
        }

        public enum HomeReportingDays
        {
            Pending = 0,
            Today = 1,
            Tomorrow = 2,
            Week = 3
        }

        public enum SimStatus : byte
        {
            Active = 1,
            DeActive = 2,
            Suspended = 3
        }

        public enum DeviceType : byte
        {
            Device = 1,
            Accessories = 2,
            Sim = 3
        }

        public enum ProductLandingPageStatus : byte
        {
            Draft = 1,
            Published = 2
        }

        public enum AppraiseType : byte
        {
            Internal = 1,
            Client = 2
        }

        public enum ComplainType : byte
        {
            Internal = 1,
            Client = 2
        }

        public enum ComplainPriority : byte
        {
            Low = 1,
            Medium = 2,
            Critical = 3
        }
        public enum ComplainPrioritys : byte
        {
            Priority = 0,
            Low = 1,
            Medium = 2,
            Critical = 3
        }

        public enum AppraisePriority : byte
        {
            Priority = 0,
            Low = 1,
            Medium = 2,
            High = 3
        }
        public enum RunningPriority : byte
        {
            [Display(Name = "Running Status")]
            RunningStatus = 0,
            Running = 1,
            Completed = 2
        }
        public enum AppraisePrioritys : byte
        {
            Low = 1,
            Medium = 2,
            High = 3
        }

        public enum BugReportStatus : byte
        {
            Pending = 1,
            Rejected = 2,
            Inprocess = 3,
            Completed = 4
        }


        public enum ProjectClosureReviewPercentage : byte
        {
            [Display(Name = "100%")]
            HundredPercent = 1,

            [Display(Name = "50-50%")]
            FiftyPercent = 2,

            [Display(Name = "Not Sure")]
            NotSure = 3,

            [Display(Name = "Not Applicable")]
            NotApplicable = 4
        }

        public enum ProjectClosureFilterType
        {
            ProjectClosed = 1,
            ProjectRestarted = 2,
            ProjectNotRestarted = 3,

            RecurringProjectClosed = 4,
            RecurringProjectRestarted = 5,

            ProjectPromising = 6,
            ProjectLessPromising = 7,
            ProjectNotSure = 8
        }

        public enum NotificationFor : byte
        {
            AdditionalSupport = 1,
            ProjectClosure = 2

        }

        public enum MomMeetingStatus
        {
            Pending = 1,
            Ongoing = 2,
            Delayed = 3,
            Completed = 4
        }
        public enum MomMeetingParticipantType
        {
            Group = 1,
            Individual = 2

        }
        public enum Tasks
        {
            AssignedToMe = 1,
            AssignedByMe = 2,
            OtherTeamMembers = 3,
            Completed = 4
        }

        public enum PMPrefernceKeys
        {
            [Display(Description = "To Display handover documents on CI document section",
                GroupName = "IsHandoverDocumentTemplateVisible",
                Name = "Handover Document Templates",
                ShortName = "bool")]
            IsHandoverDocumentTemplateVisible = 1

        }

        public enum ProjectionData : byte
        {
            Pending = 1,
            PendingAndConverted = 2,
            Converted = 3
        }

        public enum VolentryExit : byte
        {
            [Display(Name = "Only Non Volentry")]
            OnlyNonVol = 0,
            [Display(Name = "Only Volentry")]
            OnlyVol = 1
        }

        public enum ComplexityLevel : byte
        {
            Simple = 1,
            Complex = 2,
            Advanced = 3
            //[Display(Name = "Very High")]
            //VeryHigh = 4,
            //Critical = 5
        }

        public enum LibraryType : byte
        {
            [Display(Name = "Select Type")]
            Select = 0,
            [Display(Name = "Website Portfolio")]
            Website = 1,
            Design = 2,
            [Display(Name = "Mobile App Portfolio")]
            MobileApp = 3,
            // [Display(Name = "Components")]
            Component = 4,
            [Display(Name = "Technical Document")]
            Document = 5,
            Template = 6,
            [Display(Name = "Sales Kit")]
            SalesKit = 7,
            CVs = 8
        }

        public enum DesignType : byte
        {
            CRM = 1,
            Mobile = 2,
            Web = 3
        }
        public enum FileType : byte
        {
            Html = 1,
            Image = 2,
            PSD = 3,
            Zip = 4,
            Document = 11
        }
        public enum FeedbackStatus : byte
        {
            Good = 1,
            [Display(Name = "Client not satisfied")]
            ClientNotSatisfied = 2,
            Worst = 3,
            Excellent = 4,
            Other = 5
        }

        public enum ActivityDetail : byte
        {
            Free = 1,
            Paid = 2,
            All = 3
        }
        public enum ImprovementType : byte
        {
            Individual = 1,
            Organization = 2
        }
        public enum WorkingHourType
        {
            Department = 1,
            Employee = 2,
            [Display(Name = "No Planned Hours")]
            NoPlannedHours = 3
        }

        public enum WorkingHourSummaryType
        {
            Department = 1,
            Employee = 2,
            [Display(Name = "No Planned Hours")]
            NoPlannedHours = 3,
            [Display(Name = "Plan Hour < (Search Days * 8)")]
            LessThenEight = 4,
            [Display(Name = "Plan Hour == Actual Hour")]
            PlansEqualsActual = 5,
            [Display(Name = "Actual Hour > (Plan Hour + 20%)")]
            MoreThenTwentyPer = 6,
            [Display(Name = "No Actual Hours")]
            NoActualHours = 7
        }

        public enum EstimateCostType
        {
            [Display(Name = "Per Month")]
            Monthly = 1,
            [Display(Name = "Per Year")]
            Yearly = 2,
            [Display(Name = "One Time")]
            OneTime = 3,
        }
        public enum EscalationStatusType
        {
            [Display(Name = "Pending")]
            Pending = 1,
            [Display(Name = "Fixed")]
            Fixed = 2,
            [Display(Name = "Wrongly Escalated")]
            WronglyEscalated = 3,
        }
        public enum UserDesignation
        {
            ProgrammerTrainee = 1,
            AssociateProgrammer = 2,
            Programmer = 3,
            ProgrammerAnalyst = 4,
            SeniorProgrammerAnalyst = 5,
            AssociatePrincipalAnalyst = 6,
            PrincipalAnalyst = 7,
            BIArchitect = 8,
            DataArchitect = 9,
            SolutionsArchitect = 10,
            TestTrainee = 11,
            AssociateTestAnalyst = 12,
            TestAnalyst = 13,
            SrTestAnalyst = 14,
            TestArchitech = 15,
            TestManager = 16,
            QualityHead = 17,
            DeliveryHead = 18,
            BusinessAnalystTrainee = 19,
            BusinessAnalyst = 20,
            SeniorBusinessAnalyst = 21,
            BusinessArchitect = 22,
            SrBusinessArchitect = 23,
            TechnologyArchitect = 24,
            Trainee = 25,
            Executive = 26,
            SrExecutive = 27,
            Manager = 28,
            SrManager = 29,
            GeneralManager_Lead = 30,
            VicePresident = 31,
            President = 32,
            OpsTrainee = 33,
            HROpsExecutive = 34,
            HROpsSrExecutive = 35,
            HROpsLead = 36,
            HRSrManager = 37,
            HRBPLead = 38,//HRGeneralManager_Lead
            HRBPVicePresident = 39,//HRVicePresident
            HRBPAGM = 40,////HRPresident
            ContentMarketingManager = 41,
            ContentDesigner = 42,
            SocialMediaManager = 43,
            SocialMediaCoordinator = 44,
            ProductMarketingManager = 45,
            PaidAdvertisingManager = 46,
            DigitalAdvertisingManager = 47,
            PaidSearchManager = 48,
            DigitalMarketingManager = 49,
            UIDesigner = 50,
            UIDeveloper = 51,
            UIUXDeveloper = 52,
            SrUIDesigner = 53,
            SrUXDeveloper = 54,
            LeadDesigner = 55,
            CreativeHead = 56,
            NetworkEngineer = 57,
            SrNetworkEngineer = 58,
            SupportSpecialist = 59,
            NetworkAdminstrator = 60,
            ITInfrastructureManager = 61,
            CloudInfraAnalyst = 62,
            DigitalMarketingTrainee = 63,
            SocialMediaCoordinator_Analyst_SEOAnalyst = 64,
            SrSocialMediaAnalyst = 65,
            PaidAdvertisingAnalyst = 66,
            SrPaidAdvertisingAnalyst = 67,
            OperationExecutive = 68,
            LinuxSystemAdmin = 69,
            CloudConsultantSales = 70,
            CloudConsultantTechnical = 71,
            Game_MixedRealityTrainee = 72,
            Game_MixedRealityDesigner = 73,
            Game_MixedRealityDeveloper = 74,
            SrGame_MixedRealityDesigner = 75,
            SrGame_MixedRealityDeveloper = 76,
            LeadArchitect = 77,
            ARVRCreativeHead = 78,
            Director = 79,
            Others = 80,
            ProjectManager = 81,
            PMOutofIndia = 82,
            UKPM = 83,
            UKBDM = 84,
            PMOAU = 85,
            AUPM = 86,
            DataEntryOperator = 87,
            TATrainee = 88,
            HRTAExecutive = 89,
            HRTASrExecutive = 90,
            HRTALead = 91,
            HRCompExecutive = 92,//ComplianceExecutive
            HRCompSrExecutive = 93,//ComplianceSrExecutive
            HRCompLead = 94,//ComplianceLead
            TA_OpsExecutive = 95,
            TA_OpsAssistantManager = 96,
            TA_OpsLead = 97,
            TA_ComplianceExecutive = 98,
            TA_ComplianceAssociate = 99,
            TA_ComplianceLead = 100,
            HRPayrollAssociateLead = 101,//TA_Ops_PayrollAssociate
            HRPayrollLead = 102,//TA_Ops_PayrollLead
            HROpsAssociateLead = 103,
            HRCompAssociateLead = 104,
            HRTDExecutive = 105,
            HRTDSrExecutive = 106,
            HRTDAssociateLead = 107,
            HRTDLead = 108,
            HRTAAssociateLead = 109,
            HRPayrollExecutive = 110,
            HRPayrollSrExecutive = 111,
            PostSalesBusinessAnalystTrainee = 112,
            PostSalesBusinessAnalyst = 113,
            PostSalesSeniorBusinessAnalyst = 114,
            TechnicalProjectManager = 115,
            UXDeveloper = 116,
            PostSalesJuniorBusinessAnalyst = 117,
            PostSalesAssociateProjectManager = 118,
            PostSalesProjectManager = 119,
            ManagementTrainee = 120,
            PreSalesJuniorBusinessAnalyst = 121,
            SEOTrainee = 122,
            JrSEOExecutive = 123,
            JrContentWriter = 124,
            SocialMediaExecutive = 125,
            SEOExecutive = 126,
            ContentWriter = 127,
            DigitalMarketingAnalyst = 128,
            PPCSpecialist = 129,
            SrSEOExecutive = 130,
            DigitalMarketingDirector = 131
        }

        public enum LockUnlockType
        {
            [Display(Name = "All")]
            All = 2,
            [Display(Name = "Lock")]
            Lock = 1,
            [Display(Name = "Unlock")]
            Unlock = 0,
        }

        public enum TdsType
        {
            [Display(Name = "10(13A)-HRA Dedution Paid Rent More Than l Lac")]
            HRADedution13A = 3,
            [Display(Name = "24B-House Loan Interest Paid Rent More Than l Lac")]
            HouseLoan24B = 4
        }

        public enum DeductionType
        {
            [Display(Name = "AgreementTotalRentabove_1lacs")]
            AgreementTotalRentabove_1lacs = 22,
            [Display(Name = "LandlordPanAttestedCopyTotalRentabove_1lacs")]
            LandlordPanAttestedCopyTotalRentabove_1lacs = 23,
            [Display(Name = "LoanInterestCertificate")]
            LoanInterestCertificate = 24

        }

        
        public enum ExperienceLevel
        {
            [Description("Entry Level")]
            EntryLevelPrice = 1,
            [Description("1-2 Years")]
            OneToTwoPrice = 2,
            [Description("3-6 Years")]
            ThreeToSixPrice = 3,
            [Description("6-10 Years")]
            SixToTenPrice = 4,
            [Description("10+ Years")]
            TenPlusPrice = 5,
        }

        public enum CurrencyRates
        {
            AUD = 2,
            USD = 3,
            AED = 6
        }

        public enum ReminderType
        {
            [Description("One Time")]
            OneTime = 1,
            [Description("Weekly")]
            Weekly = 2,
            [Description("Biweekly")]
            Biweekly = 3,
            [Description("Monthly")]
            Monthly = 4,
        }
    }
}
