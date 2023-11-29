using EMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace UpdateTeamEmployeeCountSchedular
{
    class Program
    {
        public static IConfigurationRoot configuration;
        public static IConfiguration config;
        static void Main(string[] args)
        {
            ConfigureServices();
            UpdateTeamStatus();
        }

        private static void UpdateTeamStatus()
        {
            try
            {
                var connection = configuration["ConnectionStrings:dsmanagementConnection"];
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateTeamMembersCount", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                WriteLogFile($"===== Team Employee Count Scheduler End on Date : { DateTime.Now } =====");
            }
            catch (Exception ex)
            {
                WriteLogFile($"Error Occurred on { DateTime.Now }. Error Description :- {(ex.InnerException ?? ex).Message}\n\n===== Team Employee Count Scheduler End on Date : { DateTime.Now } =====");
            }
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
            services.AddSingleton<IConfigurationRoot>(configuration);
            //var connection = configuration["ConnectionStrings:dsmanagementConnection"];
            SiteKeys.Configure(configuration);
        }

        private static void WriteLogFile(string Description)
        {

            string filePath = $"{GetApplicationRoot()}\\TeamEmployeeUpdateLog.txt";

            StreamWriter objWrite = default(StreamWriter);

            if (File.Exists(filePath))//append  
            {
                objWrite = File.AppendText(filePath);
            }
            else//create                
                objWrite = File.CreateText(filePath);

            objWrite.WriteLine(Description + Environment.NewLine);

            objWrite.Flush();
            objWrite.Close();
            objWrite.Dispose();
        }

        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }
}
