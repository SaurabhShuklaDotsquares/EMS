using BirthdayScheduler.DAL;
using BirthdayScheduler.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BirthdayScheduler
{
   public class Program
    {
        public static IConfigurationRoot configuration;
        private static ServiceCollection serviceCollection;
        public static void Main(string[] args)
        {
            ConfigureServices(serviceCollection);
            Birthday();
        }

        #region Birthday Sender
        private static void Birthday()
        {
            int weekday = 0;
            weekday = Convert.ToInt32(DateTime.Now.DayOfWeek);

            WriteLogFile("==============================Birthday Scheduler Start  On Date : " + DateTime.Now.ToString() + "================================");

            if (weekday == 0 || weekday == 6)
            {
                WriteLogFile("Birthday Scheduler cannot run on saturday and sunday");
                WriteLogFile("=============================Birthday Scheduler End  On Date : " + DateTime.Now.ToString() + "===================================");
                return;
            }

            int MailSendCounter = 0;
            int MailNotSendCounter = 0;

            List<BirthdayUserDto> birthdayUsers = BirthdayDAL.GetUsersForBirthdaySchedular().ToList();
           
            foreach (var item in birthdayUsers)
            {
                {
                    string pmEmail = string.Empty;
                    string HRmail = string.Empty;
                    string goodName = $"{item.Title} {item.Name}";
                    #region birthday mail
                    try
                    {
                        FlexiMail mailer = new FlexiMail();
                        mailer.ValueArray = new string[]
                        {
                                goodName
                        };
                        mailer.Subject = "Birthday Wishes";
                        mailer.MailBodyManualSupply = true;
                        mailer.MailBody = mailer.GetHtml($"{GetApplicationRoot()}\\EmailTemplate\\BirthdayEmail.html");
                        mailer.To = item.EmailPersonal;
                        mailer.CC = "";
                        mailer.BCC = "";
                        mailer.From = "no-reply@dotsquares.com";
                        mailer.Send();
                    }
                    catch (Exception ex)
                    {
                        MailNotSendCounter++;
                        TraceError.ReportError(ex, SiteKey.ErrorMailTo, "Birthday");
                        WriteLogFile(" Error :-" + "Email has not been sent to: " + goodName + ". Error Description:- " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    }
                    #endregion

                    WriteLogFile("Birthday Email send to: " + goodName + " email address");
                    MailSendCounter++;
                }

            }

            WriteLogFile("Total person who received mail for Birthday :- " + MailSendCounter.ToString());

            WriteLogFile("Total person who did not received mail for Birthday :- " + MailNotSendCounter.ToString());

            WriteLogFile("=============================Birthday Scheduler End  On Date : " + DateTime.Now.ToString() + "===================================");



        }
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
            SiteKey.Configure(configuration);
        }
        private static void WriteLogFile(string Description)
        {

            string filePath = $"{GetApplicationRoot()}\\BirthdayLog.txt";

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
        #endregion
    }
}
