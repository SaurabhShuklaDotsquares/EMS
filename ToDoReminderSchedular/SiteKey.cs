using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;


namespace ToDoReminderSchedular
{
    public static class SiteKey
    {
        public static IConfigurationRoot configuration;
        private static ServiceCollection serviceCollection;
        static SiteKey()
        {
            serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
        }      
        public static string HostName => configuration["SiteKeys:HostName"];
        public static string SMTPUserName => configuration["SiteKeys:SMTPUserName"];
        public static string SMTPPassword => configuration["SiteKeys:SMTPPassword"];
        public static string SMTPPort => configuration["SiteKeys:SMTPPort"];
        public static string LogFilePath => configuration["SiteKeys:LogFilePath"];

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsetings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
        }
    }   
}
