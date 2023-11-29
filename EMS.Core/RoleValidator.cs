using System;
using System.Collections.Generic;
using System.Text;
using static EMS.Core.Enums;

namespace EMS.Core
{
    public static class RoleValidator
    {
        public static List<int> TL_RoleIds = new List<int> { (int)Enums.UserRoles.DVManagerial, (int)Enums.UserRoles.DVPManagerial, (int)Enums.UserRoles.QAManagerial, (int)Enums.UserRoles.QAPManagerial, (int)Enums.UserRoles.UIUXManagerial, (int)Enums.UserRoles.UIUXDesigner, (int)Enums.UserRoles.GamingManagerial };//TL replace
        public static List<int> DV_RoleIds = new List<int> { (int)Enums.UserRoles.DV, (int)Enums.UserRoles.GamingDesignerDeveloper, (int)Enums.UserRoles.GamingDeveloper };//SD replace
        public static List<int> BA_RoleIds = new List<int> { (int)Enums.UserRoles.BAPreSales, (int)Enums.UserRoles.BAPrePostSales, (int)Enums.UserRoles.BAPostSales };//BA
        public static int[] Multiple_RoleIds = new int[] { (int)Enums.UserRoles.DVManagerial, (int)Enums.UserRoles.DVPManagerial, (int)Enums.UserRoles.QAManagerial, (int)Enums.UserRoles.QAPManagerial, (int)Enums.UserRoles.UIUXManagerial, (int)Enums.UserRoles.UIUXDesigner, (int)Enums.UserRoles.BAPreSales, (int)Enums.UserRoles.BAPrePostSales, (int)Enums.UserRoles.BAPostSales, (int)Enums.UserRoles.DV, (int)Enums.UserRoles.GamingManagerial };
        public static List<int> QA_RoleIds = new List<int> { (int)Enums.UserRoles.QAManagerial, (int)Enums.UserRoles.QA, (int)Enums.UserRoles.QAPManagerial };//QA
        public static List<int> UIUX_RoleIds = new List<int> { (int)Enums.UserRoles.UIUXManagerial, (int)Enums.UserRoles.UIUXDesigner, (int)Enums.UserRoles.UIUXDeveloper, (int)Enums.UserRoles.UIUXFrontEndDeveloper, (int)Enums.UserRoles.UIUXMeanStackDeveloper };//DG
        public static List<int> roleIds = new List<int> { (int)Enums.UserRoles.PM, (int)Enums.UserRoles.DVManagerial, (int)Enums.UserRoles.DVPManagerial, (int)Enums.UserRoles.QAManagerial, (int)Enums.UserRoles.QAPManagerial, (int)Enums.UserRoles.UIUXManagerial, (int)Enums.UserRoles.UIUXDesigner, (int)Enums.UserRoles.BAPreSales, (int)Enums.UserRoles.BAPrePostSales, (int)Enums.UserRoles.BAPostSales, (int)Enums.UserRoles.DV };
        public static int[] SupportTeam_RoleIds = new int[] { (int)Enums.UserRoles.DV, (int)Enums.UserRoles.DVPManagerial, (int)Enums.UserRoles.DVManagerial, (int)Enums.UserRoles.QA, (int)Enums.UserRoles.QAPManagerial, (int)Enums.UserRoles.QAManagerial, (int)Enums.UserRoles.BAPreSales, (int)Enums.UserRoles.BAPrePostSales, (int)Enums.UserRoles.BAPostSales, (int)Enums.UserRoles.UIUXDesigner, (int)Enums.UserRoles.GamingDesigner, (int)Enums.UserRoles.UIUXDeveloper, (int)Enums.UserRoles.UIUXFrontEndDeveloper, (int)Enums.UserRoles.UIUXManagerial, (int)Enums.UserRoles.UIUXMeanStackDeveloper, (int)Enums.UserRoles.GamingManagerial };
        public static string[] DeveloperRoles = new string[] { ((int)Enums.UserRoles.DV).ToString(), ((int)Enums.UserRoles.DVManagerial).ToString(), ((int)Enums.UserRoles.DVPManagerial).ToString(), ((int)Enums.UserRoles.UIUXDesigner).ToString(), ((int)Enums.UserRoles.UIUXDeveloper).ToString(), ((int)Enums.UserRoles.UIUXFrontEndDeveloper).ToString(), ((int)Enums.UserRoles.UIUXManagerial).ToString(), ((int)Enums.UserRoles.UIUXMeanStackDeveloper).ToString(), ((int)Enums.UserRoles.DMCSEO).ToString(), ((int)Enums.UserRoles.DMCSMO).ToString(), ((int)Enums.UserRoles.DMCContentWriter).ToString(), ((int)Enums.UserRoles.DMCDigitalAdvertisingManager).ToString(), ((int)Enums.UserRoles.DMCDigitalMarketer).ToString(), ((int)Enums.UserRoles.DMCManagingContent).ToString(), ((int)Enums.UserRoles.DMCPaidAdvertisingManager).ToString(), ((int)Enums.UserRoles.DMCPaidSearchManager).ToString(), ((int)Enums.UserRoles.DMCPPC).ToString(), ((int)Enums.UserRoles.DMCProductMarketingManager).ToString(), ((int)Enums.UserRoles.GamingManagerial).ToString(), ((int)Enums.UserRoles.GamingDesignerDeveloper).ToString(), ((int)Enums.UserRoles.GamingDeveloper).ToString() };
        public static int[] HR_RoleIds = new int[] { (int)Enums.UserRoles.HRBP, (int)Enums.UserRoles.HROperations, (int)Enums.UserRoles.HRPayroll, (int)Enums.UserRoles.HRTA, (int)Enums.UserRoles.HRComp, (int)Enums.UserRoles.HRTD, (int)Enums.UserRoles.HRTAOps, (int)Enums.UserRoles.HRTAComp };
        public static int[] HROperations_RoleIds = new int[] { (int)Enums.UserRoles.HROperations, (int)Enums.UserRoles.HRPayroll, (int)Enums.UserRoles.HRTA, (int)Enums.UserRoles.HRComp, (int)Enums.UserRoles.HRTD, (int)Enums.UserRoles.HRTAOps, (int)Enums.UserRoles.HRTAComp };
        public static int[] HideKRA_RoleIds = new int[] { (int)Enums.UserRoles.PMO, (int)Enums.UserRoles.UKPM, (int)Enums.UserRoles.UKBDM, (int)Enums.UserRoles.PMOAU, (int)Enums.UserRoles.AUPM };
        //Designation       
        public static int[] TL_Technical_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.SeniorProgrammerAnalyst,
            (int)Enums.UserDesignation.AssociatePrincipalAnalyst,
            (int)Enums.UserDesignation.PrincipalAnalyst,
            (int)Enums.UserDesignation.BIArchitect,
            (int)Enums.UserDesignation.DataArchitect,
            (int)Enums.UserDesignation.SolutionsArchitect,
            (int)Enums.UserDesignation.TechnicalProjectManager
        };//TL_Technical
        public static int[] TL_QualityAnalyst_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.SrTestAnalyst,
            (int)Enums.UserDesignation.TestArchitech,
            (int)Enums.UserDesignation.TestManager,
            (int)Enums.UserDesignation.QualityHead,
            (int)Enums.UserDesignation.DeliveryHead
        };//TL_QualityAnalyst
        public static int[] TL_Sales_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.SeniorBusinessAnalyst,
            (int)Enums.UserDesignation.BusinessArchitect,
            (int)Enums.UserDesignation.SrBusinessArchitect,
            (int)Enums.UserDesignation.TechnologyArchitect,
            (int)Enums.UserDesignation.PostSalesSeniorBusinessAnalyst,
            (int)Enums.UserDesignation.PostSalesAssociateProjectManager,
            (int)Enums.UserDesignation.PostSalesProjectManager
        };//TL replace
        public static int[] TL_AccountsandAdmin_DesignationIds = new int[]
        {
             (int)Enums.UserDesignation.SrExecutive,
            (int)Enums.UserDesignation.Manager,
            (int)Enums.UserDesignation.SrManager,
            (int)Enums.UserDesignation.GeneralManager_Lead,
            (int)Enums.UserDesignation.VicePresident,
            (int)Enums.UserDesignation.President
        };//TL_AccountsandAdmin
        public static int[] TL_AccountsAdminSR_DesignationIds = new int[]
        {
             (int)Enums.UserDesignation.SrExecutive,
            (int)Enums.UserDesignation.Manager,
            (int)Enums.UserDesignation.SrManager,
            (int)Enums.UserDesignation.GeneralManager_Lead
        };
        public static int[] TL_HR_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.HROpsSrExecutive,
            (int)Enums.UserDesignation.HROpsLead,
            (int)Enums.UserDesignation.HRSrManager,
            (int)Enums.UserDesignation.HRBPLead,
            (int)Enums.UserDesignation.HRBPVicePresident,
            (int)Enums.UserDesignation.HRBPAGM,
            (int)Enums.UserDesignation.HRPayrollLead,
            (int)Enums.UserDesignation.HRTASrExecutive,
            (int)Enums.UserDesignation.HRTALead,
            (int)Enums.UserDesignation.HRCompLead,
            (int)Enums.UserDesignation.HRTAExecutive,
            (int)Enums.UserDesignation.HROpsExecutive,
            (int)Enums.UserDesignation.HRCompExecutive,
            (int)Enums.UserDesignation.HRPayrollExecutive,
            (int)Enums.UserDesignation.TA_OpsLead,
            (int)Enums.UserDesignation.TA_ComplianceLead,
            (int)Enums.UserDesignation.HRCompSrExecutive,
            (int)Enums.UserDesignation.HRPayrollAssociateLead,
            (int)Enums.UserDesignation.HROpsAssociateLead,
            (int)Enums.UserDesignation.HRCompAssociateLead,
            (int)Enums.UserDesignation.HRTDSrExecutive,
            (int)Enums.UserDesignation.HRTDAssociateLead,
            (int)Enums.UserDesignation.HRTDLead,
            (int)Enums.UserDesignation.HRTAAssociateLead,
            (int)Enums.UserDesignation.HRPayrollSrExecutive
        };//TL_HR
        //public static int[] TL_DigitalMarketing_DesignationIds = new int[]
        //{
        //     (int)Enums.UserDesignation.ContentMarketingManager,
        //    (int)Enums.UserDesignation.SocialMediaManager,
        //    (int)Enums.UserDesignation.ProductMarketingManager,
        //    (int)Enums.UserDesignation.PaidAdvertisingManager,
        //    (int)Enums.UserDesignation.DigitalAdvertisingManager,
        //    (int)Enums.UserDesignation.PaidSearchManager,
        //    (int)Enums.UserDesignation.DigitalMarketer,
        //    (int)Enums.UserDesignation.SrSocialMediaAnalyst,
        //    (int)Enums.UserDesignation.SrPaidAdvertisingAnalyst
        //};
        //TL_DigitalMarketing
        public static int[] TL_DigitalMarketing_DesignationIds = new int[]
        {
             (int)Enums.UserDesignation.DigitalMarketingDirector,
             (int)Enums.UserDesignation.DigitalMarketingManager

        };
        public static int[] TL_UIUX_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.SrUIDesigner,
            (int)Enums.UserDesignation.SrUXDeveloper,
            (int)Enums.UserDesignation.LeadDesigner,
            (int)Enums.UserDesignation.CreativeHead
        };//TL_UIUX
       public static int[] TL_ITInfra_DesignationIds = new int[]
       {
            (int)Enums.UserDesignation.SrNetworkEngineer,
            (int)Enums.UserDesignation.ITInfrastructureManager,
            (int)Enums.UserDesignation.CloudInfraAnalyst
       };//TL_ITInfra
       public static int[] TL_ARVRUnityGaming_DesignationIds = new int[]
       {
             (int)Enums.UserDesignation.SrGame_MixedRealityDesigner,
            (int)Enums.UserDesignation.SrGame_MixedRealityDeveloper,
            (int)Enums.UserDesignation.LeadArchitect,
            (int)Enums.UserDesignation.ARVRCreativeHead
       };//TL_AR/VR/Unity/Gaming

        //DV_Technical
        public static int[] DV_Technical_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.ProgrammerTrainee,
            (int)Enums.UserDesignation.AssociateProgrammer,
            (int)Enums.UserDesignation.Programmer,
            (int)Enums.UserDesignation.ProgrammerAnalyst,
            (int)Enums.UserDesignation.SeniorProgrammerAnalyst
        };
        public static int[] DV_QualityAnalyst_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.TestTrainee,
            (int)Enums.UserDesignation.AssociateTestAnalyst,
            (int)Enums.UserDesignation.TestAnalyst,
            (int)Enums.UserDesignation.SrTestAnalyst
        };
        public static int[] DV_UIUX_DesignationIds = new int[]
        {
             (int)Enums.UserDesignation.UIDesigner,
            (int)Enums.UserDesignation.UIDeveloper,
            (int)Enums.UserDesignation.UIUXDeveloper,
            (int)Enums.UserDesignation.SrUIDesigner,
            (int)Enums.UserDesignation.UXDeveloper,
            (int)Enums.UserDesignation.SrUXDeveloper            
        };
        public static int[] DV_ARVRUnityGaming_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.Game_MixedRealityTrainee,
            (int)Enums.UserDesignation.Game_MixedRealityDesigner,
            (int)Enums.UserDesignation.Game_MixedRealityDeveloper,
            (int)Enums.UserDesignation.SrGame_MixedRealityDesigner,
            (int)Enums.UserDesignation.SrGame_MixedRealityDeveloper
        };
        public static int[] Trainee_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.ProgrammerTrainee,
            (int)Enums.UserDesignation.TestTrainee,
            (int)Enums.UserDesignation.BusinessAnalystTrainee,
            (int)Enums.UserDesignation.Trainee,
            (int)Enums.UserDesignation.OpsTrainee,
            (int)Enums.UserDesignation.DigitalMarketingTrainee,
            (int)Enums.UserDesignation.Game_MixedRealityTrainee,
            (int)Enums.UserDesignation.TATrainee
        };

        public static int[] AllQualityAnalyst_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.TestTrainee,
            (int)Enums.UserDesignation.AssociateTestAnalyst,
            (int)Enums.UserDesignation.TestAnalyst,
            (int)Enums.UserDesignation.SrTestAnalyst,
            (int)Enums.UserDesignation.TestArchitech,
            (int)Enums.UserDesignation.TestManager,
            (int)Enums.UserDesignation.QualityHead,
            (int)Enums.UserDesignation.DeliveryHead
        };
        public static int[] AllSales_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.BusinessAnalystTrainee,
            (int)Enums.UserDesignation.BusinessAnalyst,
            (int)Enums.UserDesignation.SeniorBusinessAnalyst,
            (int)Enums.UserDesignation.BusinessArchitect,
            (int)Enums.UserDesignation.SrBusinessArchitect,
            (int)Enums.UserDesignation.TechnologyArchitect,
            (int)Enums.UserDesignation.PostSalesBusinessAnalystTrainee,
            (int)Enums.UserDesignation.PostSalesBusinessAnalyst,
            (int)Enums.UserDesignation.PostSalesSeniorBusinessAnalyst,
            (int)Enums.UserDesignation.PostSalesJuniorBusinessAnalyst,
            (int)Enums.UserDesignation.PostSalesAssociateProjectManager,
            (int)Enums.UserDesignation.PostSalesProjectManager,
            (int)Enums.UserDesignation.PreSalesJuniorBusinessAnalyst
        };
        public static int[] AllUIUX_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.UIDesigner,
            (int)Enums.UserDesignation.UIDeveloper,
            (int)Enums.UserDesignation.UIUXDeveloper,
            (int)Enums.UserDesignation.SrUIDesigner,
            (int)Enums.UserDesignation.UXDeveloper,
            (int)Enums.UserDesignation.SrUXDeveloper,
            (int)Enums.UserDesignation.LeadDesigner,
            (int)Enums.UserDesignation.CreativeHead
        };
        public static int[] AllHR_DesignationIds = new int[]
        {
            (int)Enums.UserDesignation.OpsTrainee,
            (int)Enums.UserDesignation.HROpsExecutive,
            (int)Enums.UserDesignation.HROpsSrExecutive,
            (int)Enums.UserDesignation.HROpsLead,
            (int)Enums.UserDesignation.HRSrManager,
            (int)Enums.UserDesignation.HRBPLead,
            (int)Enums.UserDesignation.HRBPVicePresident,
            (int)Enums.UserDesignation.HRBPAGM,
            (int)Enums.UserDesignation.TATrainee,
            (int)Enums.UserDesignation.HRTAExecutive,
            (int)Enums.UserDesignation.HRTASrExecutive,
            (int)Enums.UserDesignation.HRTALead,
            (int)Enums.UserDesignation.HRCompExecutive,
            (int)Enums.UserDesignation.HRCompSrExecutive,
            (int)Enums.UserDesignation.HRCompLead,
            (int)Enums.UserDesignation.TA_OpsLead,
            (int)Enums.UserDesignation.TA_ComplianceLead,
            (int)Enums.UserDesignation.HRPayrollAssociateLead,
            (int)Enums.UserDesignation.HRPayrollLead,
            (int)Enums.UserDesignation.HROpsAssociateLead,
            (int)Enums.UserDesignation.HRCompAssociateLead,
            (int)Enums.UserDesignation.HRTDExecutive,
            (int)Enums.UserDesignation.HRTDSrExecutive,
            (int)Enums.UserDesignation.HRTDAssociateLead,
            (int)Enums.UserDesignation.HRTDLead,
            (int)Enums.UserDesignation.HRTAAssociateLead,
            (int)Enums.UserDesignation.HRPayrollExecutive,
            (int)Enums.UserDesignation.HRPayrollSrExecutive,
            (int)Enums.UserDesignation.ManagementTrainee
        };
        public static int[] HRWithOutTrainee_DesignationIds = new int[]
        {            
            (int)Enums.UserDesignation.HROpsExecutive,
            (int)Enums.UserDesignation.HROpsSrExecutive,
            (int)Enums.UserDesignation.HROpsLead,
            (int)Enums.UserDesignation.HRSrManager,
            (int)Enums.UserDesignation.HRBPLead,
            (int)Enums.UserDesignation.HRBPVicePresident,
            (int)Enums.UserDesignation.HRBPAGM,
            (int)Enums.UserDesignation.HRTAExecutive,
            (int)Enums.UserDesignation.HRTASrExecutive,
            (int)Enums.UserDesignation.HRTALead,
            (int)Enums.UserDesignation.HRCompExecutive,
            (int)Enums.UserDesignation.HRCompSrExecutive,
            (int)Enums.UserDesignation.HRCompLead,
            (int)Enums.UserDesignation.TA_OpsLead,
            (int)Enums.UserDesignation.TA_ComplianceLead,
            (int)Enums.UserDesignation.HRPayrollAssociateLead,
            (int)Enums.UserDesignation.HRPayrollLead,
            (int)Enums.UserDesignation.HROpsAssociateLead,
            (int)Enums.UserDesignation.HRCompAssociateLead,
            (int)Enums.UserDesignation.HRTDExecutive,
            (int)Enums.UserDesignation.HRTDSrExecutive,
            (int)Enums.UserDesignation.HRTDAssociateLead,
            (int)Enums.UserDesignation.HRTDLead,
            (int)Enums.UserDesignation.HRTAAssociateLead,
            (int)Enums.UserDesignation.HRPayrollExecutive,
            (int)Enums.UserDesignation.HRPayrollSrExecutive
        };
        public static int[] ActivityDepartment = new int[]
        {
             (int)ProjectDepartment.DotNetDevelopment,
             (int)ProjectDepartment.SEO,
             (int)Enums.ProjectDepartment.PHPDevelopment,
             (int)Enums.ProjectDepartment.MobileApplication,
             (int)Enums.ProjectDepartment.HubSpot,
             (int)Enums.ProjectDepartment.BusinessAnalyst,
             (int)Enums.ProjectDepartment.WebDesigning,
             (int)Enums.ProjectDepartment.QualityAnalyst
        };

        //public static int[] TL_DesignationIds = new int[]
        //{
        //    (int)Enums.UserDesignation.SeniorProgrammerAnalyst,
        //    (int)Enums.UserDesignation.AssociatePrincipalAnalyst,
        //    (int)Enums.UserDesignation.PrincipalAnalyst,
        //    (int)Enums.UserDesignation.BIArchitect,
        //    (int)Enums.UserDesignation.DataArchitect,
        //    (int)Enums.UserDesignation.SolutionsArchitect,
        //    (int)Enums.UserDesignation.SrTestAnalyst,
        //    (int)Enums.UserDesignation.TestArchitech,
        //    (int)Enums.UserDesignation.TestManager,
        //    (int)Enums.UserDesignation.QualityHead,
        //    (int)Enums.UserDesignation.DeliveryHead,
        //    (int)Enums.UserDesignation.ContentMarketingManager,
        //    (int)Enums.UserDesignation.SocialMediaManager,
        //    (int)Enums.UserDesignation.SrPaidAdvertisingAnalyst,
        //    (int)Enums.UserDesignation.ProductMarketingManager,
        //    (int)Enums.UserDesignation.PaidAdvertisingManager,
        //    (int)Enums.UserDesignation.DigitalAdvertisingManager,
        //    (int)Enums.UserDesignation.PaidSearchManager,
        //    (int)Enums.UserDesignation.DigitalMarketer,
        //    (int)Enums.UserDesignation.SrUIDesigner,
        //    (int)Enums.UserDesignation.LeadDesigner,
        //    (int)Enums.UserDesignation.SrUXDeveloper,
        //    (int)Enums.UserDesignation.CreativeHead,
        //    (int)Enums.UserDesignation.NetworkAdminstrator,
        //    (int)Enums.UserDesignation.ITInfrastructureManager,
        //    (int)Enums.UserDesignation.SrGame_MixedRealityDesigner,
        //    (int)Enums.UserDesignation.LeadArchitect,
        //    (int)Enums.UserDesignation.SrGame_MixedRealityDeveloper,
        //    (int)Enums.UserDesignation.ARVRCreativeHead
        //};//TL replace 

    }
}
