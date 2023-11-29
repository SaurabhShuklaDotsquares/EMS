using EMS.Core;
using EMS.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToDoReminderSchedular
{
    class Program
    { 
        static void Main(string[] args)
        {
            GetAppSettingsFile();
            TaskSchedular();
        }

        static void GetAppSettingsFile()
        {
            LoadConnectionString();
        }

        private static void LoadConnectionString()
        {           
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            Console.WriteLine(configuration.GetConnectionString("dsmanagementConnection"));           
        }

        static void TaskSchedular()
        {
            try
            {
                DateTime lastDate = DateTime.Today.AddDays(1);
                using (var db = new db_dsmanagementnewContext())
                {
                    var taskList = db.Task.Where(t => t.TaskEndDate == lastDate && !t.ReminderEmailSent &&
                                   t.TaskStatusID != (int)Enums.TaskStatusType.Closed &&
                                   t.TaskStatusID != (int)Enums.TaskStatusType.Completed)
                       .ToList();

                    if (taskList != null && taskList.Any())
                    {
                        foreach (var item in taskList)
                        {
                            try
                            {
                                var users = item.TaskAssignedToes.Where(x => !string.IsNullOrWhiteSpace(x.UserLogin.EmailOffice))
                                                                .Select(x => x.UserLogin).ToList();
                                if (users.Any())
                                {
                                    var assignedUserEmail = string.Join(";", users.Select(x => x.EmailOffice));

                                    FlexiMail mailer = new FlexiMail();
                                    mailer.ValueArray = new string[]
                                    {
                                        users.Count > 1 ? "All" : users.First().Name,
                                        item.TaskName,
                                        item.TaskEndDate.ToFormatDateString(),
                                        item.UserLogin.Name,
                                    };
                                    mailer.Subject = $"Task Reminder : {item.TaskName}";
                                    mailer.MailBodyManualSupply = true;
                                    mailer.MailBody = mailer.GetHtml($"{GetApplicationRoot()}\\EmailTemplate\\TaskReminderEmail.html");
                                    mailer.To = assignedUserEmail;
                                    mailer.CC = "";
                                    mailer.BCC = "";
                                    mailer.From = "no-reply@dotsquares.com";
                                    mailer.Send();                                   
                                    var entity = db.Task.Find(item.TaskID);
                                    if (entity != null)
                                    {
                                        entity.ReminderEmailSent = true;
                                        db.SaveChanges();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteLogFile(ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogFile(ex.Message);
            }
        }

        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        private static void WriteLogFile(string Description)
        {

            string filePath = $"{GetApplicationRoot()}\\LogTaskReminder.txt";

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
    }
}
