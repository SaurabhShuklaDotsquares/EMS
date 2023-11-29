using EMS.Core;
using EMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TimesheetReminderSchedular.DAL;
using ToDoReminderSchedular;

namespace TimesheetReminderSchedular
{
    class Program
    {
        public static IConfigurationRoot configuration;
        private static ServiceCollection serviceCollection;
        static void Main(string[] args)
        {
            ConfigureServices(serviceCollection);
            TimeSheetReminder();
        }

        private static void TimeSheetReminder()
        {
            int weekday = 0;
            weekday = Convert.ToInt32(DateTime.Now.DayOfWeek);
            WriteLogFile("==============================TimeSheet Scheduler Start  On Date : " + DateTime.Now.ToString() + "================================");

            if (weekday == 0 || weekday == 6)
            {
                WriteLogFile("TimeSheet Scheduler cannot run on saturday and sunday");
                WriteLogFile("=============================TimeSheet Scheduler End  On Date : " + DateTime.Now.ToString() + "===================================");
                return;
            }

            int MailSendCounter = 0;
            int MailNotSendCounter = 0;

            List<UserSchedulerDto> users = TimeSheetDAL.GetUsersForTimeSheetSchedular().ToList();
            List<UserSchedulerDto> filteredUsers = users.Where(t => (t.TimeSheetDate.HasValue ? t.TimeSheetDate < DateTime.Now : true) && t.PMUid > 0).ToList();
            List<Preference> prefrences = TimeSheetDAL.GetAllPrefrences();
            List<OfficialLeave> officialLeaves = TimeSheetDAL.GetOfficalLeave();
            if (prefrences != null && prefrences.Any())
            {
                filteredUsers.ForEach(u => u.TimesheetPrefrence = (prefrences.FindAll(pr => pr.pmid == u.PMUid).FirstOrDefault() != null && prefrences.FindAll(pr => pr.pmid == u.PMUid).FirstOrDefault().TimeSheetDay.HasValue ? prefrences.FindAll(pr => pr.pmid == u.PMUid).FirstOrDefault().TimeSheetDay.Value : 1));
            }
            else { WriteLogFile("Line No - 37 :: Not have preferences"); }
            foreach (var item in filteredUsers)
            {
                try
                {
                    if (!item.TimesheetPrefrence.HasValue)
                        WriteLogFile("Line No - 49 :: item.TimesheetPrefrence does not have value");

                    int Timesheet = 0;
                    Timesheet = item.TimesheetPrefrence.HasValue ? item.TimesheetPrefrence.Value : 1; //Adjusting Sat and Sun;
                    item.TimesheetPrefrence = weekday <= Timesheet ? Timesheet + 2 : Timesheet;
                    bool sent = false;

                    if (!item.TimeSheetDate.HasValue)
                        sent = true;
                    else
                    {
                        var officialLeavesCount = officialLeaves.Where(ol => ol.LeaveDate >= item.TimeSheetDate && ol.LeaveDate <= DateTime.Now.Date && ol.CountryId == (item.Role == 19 || item.Role == 14 ? 2 : 1)).Count();
                        if ((DateTime.Now.AddDays(-1).Subtract(item.TimeSheetDate.Value).Days) - (officialLeavesCount) >= item.TimesheetPrefrence)
                        {
                            if (item.UserLeaves != null && item.UserLeaves.Any())//if (item.UserLeaves.Count > 0)
                            {
                                item.LeaveStartDate = item.UserLeaves.FindAll(ul => ul.StartDate > item.TimeSheetDate).OrderBy(m => m.StartDate).FirstOrDefault() != null ? item.UserLeaves.Where(ul => ul.StartDate > item.TimeSheetDate).OrderBy(m => m.StartDate).FirstOrDefault().StartDate : (DateTime?)null;
                                item.LeaveEndDate = item.UserLeaves.FindAll(ul => ul.EndDate > item.TimeSheetDate).OrderByDescending(m => m.EndDate).FirstOrDefault() != null ? item.UserLeaves.Where(ul => ul.EndDate > item.TimeSheetDate).OrderByDescending(m => m.EndDate).FirstOrDefault().EndDate : (DateTime?)null;

                                if (item.LeaveEndDate.HasValue && item.TimeSheetDate < item.LeaveEndDate)
                                {
                                    if (item.LeaveStartDate.HasValue && item.LeaveStartDate.Value.Date < DateTime.Now.Date)
                                    {
                                        if (DateTime.Now.Subtract(item.LeaveEndDate.Value).Days > item.TimesheetPrefrence)
                                        {
                                            if (item.TimeSheetDate.Value.AddDays(1) == item.LeaveStartDate)
                                            {
                                                if (item.LeaveEndDate.Value.DayOfWeek == DayOfWeek.Friday || item.LeaveEndDate.Value.DayOfWeek == DayOfWeek.Saturday)
                                                {
                                                    int daysUntilMonday = ((int)DayOfWeek.Monday - (int)item.LeaveEndDate.Value.DayOfWeek + 7) % 7;
                                                    item.LeaveEndDate = item.LeaveEndDate.Value.AddDays(daysUntilMonday - 1);
                                                }
                                                item.TimeSheetDate = item.LeaveEndDate;
                                            }
                                            sent = true;
                                        }                                        
                                        else
                                        {
                                            //WriteLogFile("Email not send to: " + item.UserName + " email address. AS the person was on leave for date :- " + item.TimeSheetDate);
                                            string startdate = Convert.ToString(item.LeaveStartDate != null ? item.LeaveStartDate : (DateTime?)null);
                                            string enddate = Convert.ToString(item.LeaveEndDate != null ? item.LeaveEndDate : (DateTime?)null);
                                            WriteLogFile("Email not send to: " + item.UserName + " email address. As the person was on leave from  :- " + Convert.ToDateTime(startdate).ToString("dd/MM/yyyy") + " to " + Convert.ToDateTime(enddate).ToString("dd/MM/yyyy"));
                                            MailNotSendCounter++;
                                            sent = false;
                                        }
                                    }
                                    else
                                    {
                                        sent = true;
                                    }
                                }
                                if (officialLeavesCount > 0)
                                {
                                    item.TimeSheetDate = item.LeaveEndDate.HasValue ? item.LeaveEndDate.Value.AddDays(officialLeavesCount) : DateTime.Now.AddDays(officialLeavesCount * -1);
                                    sent = true;
                                }
                                else
                                {                                    
                                    sent = true;
                                }
                            }
                            else
                            {

                                sent = true;

                            }
                        }
                    }
                    if (sent)
                    {
                        string pmEmail = string.Empty;
                        string HRmail = string.Empty;
                        string TimeSheetmail = string.Empty;
                        if (prefrences != null && prefrences.Any())
                        {

                            TimeSheetmail = prefrences.FindAll(u => u.pmid == item.PMUid).FirstOrDefault() != null ? prefrences.FindAll(u => u.pmid == item.PMUid).FirstOrDefault().TimeSheetEmail : "";
                            try
                            {
                                FlexiMail mailer = new FlexiMail();
                                mailer.ValueArray = new string[]
                                {
                                item.UserName,
                                item.TimeSheetDate.Value.ToString()
                                };
                                mailer.Subject = $"TimeSheet not filled";
                                mailer.MailBodyManualSupply = true;
                                mailer.MailBody = mailer.GetHtml($"{GetApplicationRoot()}\\EmailTemplate\\TimeSheetEmail.html");
                                mailer.To = item.UserEmail;
                                mailer.CC = "";
                                mailer.BCC = "";
                                mailer.From = "no-reply@dotsquares.com";
                                mailer.Send();
                            }
                            catch (Exception ex)
                            {
                                MailNotSendCounter++;
                                TraceError.ReportError(ex, SiteKey.ErrorMailTo, "TimeSheet");
                                WriteLogFile("Line No. 122 :: Error :-" + "Email has not been sent to: " + item.UserName + ". Error Description:- " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                            }
                        }
                        WriteLogFile("Email send to: " + item.UserName + " email address. For time sheet not filled on date:- " + Convert.ToDateTime(item.TimeSheetDate).ToString("dd/MM/yyyy") + (TimeSheetmail == "" ? ". Email has not been sent to CC Mail as didn't get timesheet(CC) email ID in prefrences." : ""));
                        MailSendCounter++;
                    }
                }
                catch (Exception ex)
                {
                    MailNotSendCounter++;
                    TraceError.ReportError(ex, SiteKey.ErrorMailTo, "TimeSheet");
                    WriteLogFile("Line No. 138 :: Error:-" + "Email has not been sent to: " + item.UserName + ". Error Description:- " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                }

            }
            // WriteLogFile("Total person who fill timesheet regularly :- " + timesheetfillcounter.ToString());
            WriteLogFile("Total person who received mail for timesheet :- " + MailSendCounter.ToString());
            WriteLogFile("Total person who are on leave :- " + MailNotSendCounter.ToString());

            WriteLogFile("=============================TimeSheet Scheduler End  On Date : " + DateTime.Now.ToString() + "===================================");
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

            string filePath = $"{GetApplicationRoot()}\\TimeSheetLog.txt";

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
