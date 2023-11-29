using EMS.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RemovePastActivityScheduler
{
    class Program
    {
        public static IConfigurationRoot configuration;
        static void Main(string[] args)
        {
            ConfigureServices();
            RemovePastActivity();
        }

        private static void RemovePastActivity()
        {
            try
            {
                WriteLogFile($"===== Remove Past Activity Scheduler Start on Date : { DateTime.Now } =====");

                DateTime dtNow = DateTime.Now.AddDays(Convert.ToInt32(SiteKeys.ActivityBeforeDays)).Date;
                int activityCount = 0;

                using (var db = new db_dsmanagementnewContext())
                {
                    var userActivities = db.UserActivity.Where(D => D.DateAdded != null && D.DateAdded.Value <= dtNow).ToList();
                    activityCount = userActivities.Count;

                    if (activityCount > 0)
                    {
                        db.UserActivity.RemoveRange(userActivities);
                        db.SaveChanges();
                    }
                }
                WriteLogFile($"Removed {activityCount} activities\n\n===== Remove Past Activity Scheduler End on Date : { DateTime.Now } =====");
            }
            catch (Exception ex)
            {
                WriteLogFile($"Error Occurred on { DateTime.Now }. Error Description :- {(ex.InnerException ?? ex).Message}\n\n===== Remove Past Activity Scheduler End on Date : { DateTime.Now } =====");
            }
        }

        private static void ConfigureServices()
        {           
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();            
            SiteKeys.Configure(configuration);
        }

        private static void WriteLogFile(string Description)
        {

            string filePath = $"{GetApplicationRoot()}\\PastActivityLog.txt";

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
