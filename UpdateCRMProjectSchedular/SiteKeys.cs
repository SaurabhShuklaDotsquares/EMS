using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateCRMProjectSchedular
{
    public static class SiteKeys
    {
        private static IConfigurationRoot _configuration;
        public static void Configure(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }
        public static string HostName => _configuration["SiteKeys:HostName"];
        public static string SMTPUserName => _configuration["SiteKeys:SMTPUserName"];
        public static string SMTPPassword => _configuration["SiteKeys:SMTPPassword"];
        public static string SMTPPort => _configuration["SiteKeys:SMTPPort"];
        public static string LogFilePath => _configuration["SiteKeys:LogFilePath"];
        public static string ErrorMailTo => _configuration["SiteKeys:ErrorMailTo"];
        public static string CRMAPIOld => _configuration["SiteKeys:crmapiOld"] + "projectdetail?type=projects&userid=";
        public static string FromName => _configuration["SiteKeys:FromName"];
        public static string EmailFrom => _configuration["SiteKeys:EmailFrom"];
        public static Boolean IsLive => Convert.ToBoolean(_configuration["SiteKeys:IsLive"]);
    }
}
