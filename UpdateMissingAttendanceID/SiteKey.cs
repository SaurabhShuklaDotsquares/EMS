using Microsoft.Extensions.Configuration;

namespace UpdateMissingAttendanceID
{
    public static class SiteKey
    {
        private static IConfigurationRoot _configuration;
        public static void Configure(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }
        public static string HrmServiceURL => _configuration["HrmServiceURL"];
        public static string HrmApiKey => _configuration["hrmapikey"];
        public static string HrmApiPassword => _configuration["Hrmapipassword"];
    }
}
