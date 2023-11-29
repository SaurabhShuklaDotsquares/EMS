using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.LIBS
{
    public class SiteKeys
    {
        private static IConfiguration _configuration;
        public static void Configure(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public static string HostName => _configuration["SiteKeys:HostName"];
        public static string SMTPUserName => _configuration["SiteKeys:SMTPUserName"];
        public static string SMTPPassword => _configuration["SiteKeys:SMTPPassword"];
        public static string SMTPPort => _configuration["SiteKeys:SMTPPort"];
        public static string DomainName => _configuration["SiteKeys:DomainName"];
        public static string From => _configuration["SiteKeys:From"];
        public static string HREmailId => _configuration["SiteKeys:HREmailId"];


        // for pm data update keys

        public static string LogFilePath => _configuration["SiteKeys:LogFilePath"];
        public static string ErrorMailTo => _configuration["SiteKeys:ErrorMailTo"];
        public static string CRMAPI => _configuration["SiteKeys:crmapiOld"] + "projectdetail?type=projects&userid=";
        public static string FromName => _configuration["SiteKeys:FromName"];
        public static string EmailFrom => _configuration["SiteKeys:EmailFrom"];
        public static Boolean IsLive => Convert.ToBoolean(_configuration["SiteKeys:IsLive"]);
        public static string Encryption_Key => _configuration["SiteKeys:Encryption_Key"];
        public static Boolean IsSaralLive => Convert.ToBoolean(_configuration["SiteKeys:IsSaralLive"]);

    }
}
