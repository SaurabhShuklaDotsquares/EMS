using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using UpdateMissingAttendanceID.DAL;
using UpdateMissingAttendanceID.Model;

namespace UpdateMissingAttendanceID
{
    class Program
    {
        public static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            ConfigureServices();
            UpdateMissingAttendance();
        }

        private static void UpdateMissingAttendance()
        {
            int weekday = 0;
            weekday = Convert.ToInt32(DateTime.Now.DayOfWeek);
            WriteLogFile("==============================Update Missing Attendance Id Scheduler Start  On Date : " + DateTime.Now.ToString() + "================================");

            if (weekday == 0 || weekday == 6)
            {
                WriteLogFile("Update Missing Attendance Id Scheduler cannot run on saturday and sunday");
                WriteLogFile("=============================Update Missing Attendance Id Scheduler End  On Date : " + DateTime.Now.ToString() + "===================================");

                return;
            }

            List<HRMRequestDataModel> missingAttendanceEmails = MissingAttendaceDAL.GetUsersForMissingAttendanceScheduler().ToList();

            var hrmResponse = new HRMResponseDataModel();

            if (missingAttendanceEmails != null && missingAttendanceEmails.Any())
            {
                try
                {
                    hrmResponse = GetAttendanceId(missingAttendanceEmails);
                }
                catch (Exception ex)
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    WriteLogFile($"Line No. 53 :: Error:- Error calling HRM API. Error Description:- {message}");

                    hrmResponse = null;
                }

                if (hrmResponse != null)
                {
                    if (hrmResponse.Status.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                    {
                        WriteLogFile("Line No - 62 :: Status of HRM Response is False");
                    }

                    if (!hrmResponse.Message.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
                    {
                        WriteLogFile($"Line No - 67 :: HRM Response Message is {hrmResponse.Message}");
                    }

                    var errorEmails = MissingAttendaceDAL.UpdateAttendancereferenceId(hrmResponse.data);

                    if (errorEmails != null && errorEmails.Any())
                    {
                        errorEmails.ForEach(
                            e => WriteLogFile(
                                $"Line No. 76 :: Error:- Attendace Reference Id not set for: {e.email}. Error Description:- {e.ErrorMessage}"
                                ));
                    }
                }
            }
            else
            {
                WriteLogFile("Line No - 82: Email Ids not found");
            }

            WriteLogFile("=============================Update Missing Attendance Id Scheduler End  On Date : " + DateTime.Now.ToString() + "===================================");
        }

        private static void ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            configuration = builder.Build();

            SiteKey.Configure(configuration);
        }

        private static HRMResponseDataModel GetAttendanceId(List<HRMRequestDataModel> request)
        {
            PostManager postRequest = new PostManager("http://uathrm.projectstatus.co.uk/webservice/attendanceReference");
            postRequest.AddHeader("Hrmapikey", "ServiceUser");
            postRequest.AddHeader("Hrmapipassword", "12345");

            var response = postRequest.PostData<List<HRMRequestDataModel>, HRMResponseDataModel>(request);

            return response;
        }

        private static void WriteLogFile(string Description)
        {

            string filePath = $"{GetApplicationRoot()}\\MissingAttendance.txt";

            StreamWriter objWrite = default(StreamWriter);

            if (File.Exists(filePath))//append  
            {
                objWrite = File.AppendText(filePath);
            }
            else//create
            {
                objWrite = File.CreateText(filePath);
            }

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
