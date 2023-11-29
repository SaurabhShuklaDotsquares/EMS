using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace EMS.Web.Code.LIBS
{
    public class SiteKey
    {
        private static IConfigurationSection _configuration;

        public static void Configure(IConfigurationSection configuration)
        {
            _configuration = configuration;
        }

        public static string DomainName => _configuration["DomainName"];
        public static string DomainNameInternalIP => _configuration["DomainNameInternalIP"];
        public static int AshishTeamPMUId => int.Parse(_configuration["AshishTeamPMUId"]);
        public static string SMTPHost => _configuration["SMTPHost"];
        public static string SMTPPort => _configuration["SMTPPort"];
        public static string SMTPUserName => _configuration["SMTPUserName"];
        public static string SMTPPassword => _configuration["SMTPPassword"];
        public static string From => _configuration["From"];
        public static string To => _configuration["To"];
        public static string CC => _configuration["CC"];
        public static string BCC => _configuration["BCC"];
        public static string RefreshActivity => _configuration["RefreshActivity"];
        //public static string projectclosuretoemail => _configuration["projectclosuretoemail"];
        public static string ProjectClosureClientBadgeCCEmail => _configuration["ProjectClosureClientBadgeCCEmail"];
         
       // public static string escalatedtoemail => _configuration["escalatedtoemail"];
        public static string CrmProjectList => $"{_configuration["crmapiOld"]}projectdetail?type=projects&userid=";
        //public static string CrmInvoiceList => $"{_configuration["crmapiOld"]}updateInvoices?type=invoices&userid=@USERID&apipass=@APIPASSWORD&date=@INVOICEDATE";
        public static string CrmInvoices => $"{_configuration["CRMApi"]}updateInvoices";
        public static string InvoiceDays => _configuration["InvoiceDays"];
        public static string UKDeveloperUID => _configuration["UKDeveloperUID"];
        public static string UKPMVirtualDeveloperID => _configuration["UKPMVirtualDeveloperID"];
        public static string CRMApiWorkingHourUrl => _configuration["CRMApiWorkingHourUrl"];
        public static string CRMApiLeadNotesUrl => $"{_configuration["CRMApi"]}getLeadNotes";
        public static string AccessAllLeads => _configuration["AccessAllLeads"];
        public static string DailyThought1 => _configuration["DailyThought1"];
        public static string DailyThought2 => _configuration["DailyThought2"];

        public static string LogFilePath => _configuration["LogFilePath"];
        public static string LocalDirectory => _configuration["LocalDirectory"];
        public static string ScriptTagFormat => "<script src=\"{0}?v=" + DateTime.Now.Ticks + "\" type=\"text/javascript\"></script>";
        public static string StyleTagFormat => "<link href=\"{0}?v=" + DateTime.Now.Ticks + "\" rel=\"stylesheet\" />";
        public static string HrmServiceURL => _configuration["HrmServiceURL"];
        public static string HrmApiKey => _configuration["hrmapikey"];
        public static string HrmApiPassword => _configuration["Hrmapipassword"];
        public static bool IsLive => Convert.ToBoolean(_configuration["IsLive"] ?? "true");
        public static string CRMApiProjectClosureUpdateUrl => _configuration["CRMApiProjectClosureUpdateUrl"];
        public static string CRMApiProjectClosureStatusId => _configuration["CRMApiProjectClosureStatusId"];
        public static string CRMApiUser => _configuration["CRMApiUser"];
        public static string CRMApiPassword => _configuration["CRMApiPassword"];
        public static string IsServiceLive => _configuration["isServiceLive"];
        public static string Size => _configuration["size"];
        public static string Extension => _configuration["extension"];
        public static string ImageExtension => _configuration["ImageExtension"];
        public static string DesignImageExtension => _configuration["DesignImageExtension"];
        public static string PsdImageExtension => _configuration["PsdImageExtension"];
        public static string UploadResumeFolderPath=> Path.Combine(ContextProvider.HostEnvironment.WebRootPath, "Upload", "Resume");


        public static bool IsPMSClosureReportLive => Convert.ToBoolean(_configuration["IsPMSClosureReportLive"] ?? "true");
        public static string PMSApiAddClosureReportUrl => _configuration["PMSApiAddClosureReportUrl"];
        
        public static string PMSApiKey => _configuration["PMSApiKey"];
        public static string PMSApiPassword => _configuration["PMSApiPassword"];
        public static string AttendenceId => _configuration["AttendenceId"];

        public static string DocumentForAshishTeamOnly=> _configuration["DocumentForAshishTeamOnly"];
        public static string HrmAbscondServiceURL => $"{_configuration["HrmServiceURL"]}/setempstatusabscond";

        public static string PMSMemberListWithTimeLogServiceApiURL => $"{_configuration["PMSServiceApiURL"]}MemberListWithTimeLog";
        public static string Encryption_Key => _configuration["Encryption_Key"];

        public static string UKAUUserIDToShowAshishTeamActivity => _configuration["UKAUUserIDToShowAshishTeamActivity"];

        public static string AllowDeviceUserId => _configuration["AllowDeviceUserId"];

        public static string UpdateDeveloperInfo => $"{_configuration["CRMApi"]}updateDeveloperInfo";
        public static string HREmailId => _configuration["HREmailId"];

        public static string PMSUserUpdateServiceApiURL => _configuration["PMSServiceApiURL"];
        public static string AccountsEmailId => _configuration["AccountsEmailId"];
        
        public static bool IsSaralLive => Convert.ToBoolean(_configuration["IsSaralLive"] ?? "true");
        //{
        //    get
        //    {
        //        string folderPath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, "Upload", "Resume");
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }
        //        return folderPath;
        //    }
        //}
    }
}