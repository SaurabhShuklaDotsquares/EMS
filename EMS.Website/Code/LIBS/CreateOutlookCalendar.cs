using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Text;

namespace EMS.Web.Code.LIBS
{
    public class CreateOutlookCalendar
    {
        public static string CreateEventICSFile(string organizerName, string organizerEmail, string subject, string location, DateTime startDate, DateTime enddate, string desc)
        {
            string filePath = string.Empty;

            try
            {
                string uid = Guid.NewGuid().ToString();
                string path = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, "/Content/ICSFiles/");
                filePath = $"{path}Event_{uid}_{DateTime.Now.Ticks}.ics"; //path + "Event_" + EmpName +DateTime.Now.Ticks.ToString()+"_Leave"+ ".ics";

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                StreamWriter writer = new StreamWriter(filePath);

                writer.WriteLine("BEGIN:VCALENDAR");
                writer.WriteLine("PRODID: -//Microsoft Corporation//Outlook 14.0 MIMEDIR//EN");
                writer.WriteLine("VERSION:2.0");
                writer.WriteLine("METHOD:PUBLISH");
                writer.WriteLine("X-MS-OLK-FORCEINSPECTOROPEN:TRUE");
                writer.WriteLine("BEGIN:VEVENT");
                writer.WriteLine("CLASS:PUBLIC");
                writer.WriteLine("DESCRIPTION:" + desc);
                writer.WriteLine("X-ALT-DESC;FMTTYPE=text/html:" + desc);

                string startDateAndTime = GetFormatedDate(startDate) + "T" + GetFormattedTime(Convert.ToDateTime(startDate).ToString("HH:mm"));
                string endDateAndTime = GetFormatedDate(enddate) + "T" + GetFormattedTime(Convert.ToDateTime(enddate).ToString("HH:mm"));

                writer.WriteLine("DTSTART:" + startDateAndTime);
                writer.WriteLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ"));
                writer.WriteLine("DTEND:" + endDateAndTime);
                writer.WriteLine("ORGANIZER;CN=" + organizerName + ":mailto:" + organizerEmail);
                writer.WriteLine("SUMMARY:" + subject);
                writer.WriteLine("LOCATION:" + location);
                writer.WriteLine("PRIORITY:5");
                writer.WriteLine("SEQUENCE:0");
                writer.WriteLine("SUMMARY:sum");
                writer.WriteLine("TRANSP:OPAQUE");
                writer.WriteLine("UID:{0}", uid);
                writer.WriteLine("X-MICROSOFT-CDO-BUSYSTATUS:BUSY");
                writer.WriteLine("X-MICROSOFT-CDO-IMPORTANCE:1");
                writer.WriteLine("X-MICROSOFT-DISALLOW-COUNTER:FALSE");
                writer.WriteLine("X-MS-OLK-AUTOFILLLOCATION:FALSE");
                writer.WriteLine("X-MS-OLK-CONFTYPE:0");
                writer.WriteLine("BEGIN:VALARM");
                writer.WriteLine("TRIGGER:-PT15M");
                writer.WriteLine("ACTION:DISPLAY");
                writer.WriteLine("DESCRIPTION:Reminder");
                //writer.WriteLine("ORGANIZER:MAILTO: test@dstest.com");

                writer.WriteLine("END:VALARM");
                writer.WriteLine("END:VEVENT");
                writer.WriteLine("END:VCALENDAR");
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
            catch { }

            return filePath;
        }

        private static string GetFormatedDate(DateTime date)
        {
            var yy = date.Year.ToString(CultureInfo.InvariantCulture);
            string mm;
            string dd;

            if (date.Month < 10 && date.Month.ToString().Length == 1) mm = "0" + date.Month.ToString(CultureInfo.InvariantCulture);
            else mm = date.Month.ToString(CultureInfo.InvariantCulture);

            if (date.Day < 10 && date.Day.ToString().Length == 1) dd = "0" + date.Day.ToString(CultureInfo.InvariantCulture);
            else dd = date.Day.ToString(CultureInfo.InvariantCulture);

            return yy + mm + dd;
        }

        private static string GetFormattedTime(string time)
        {
            var times = time.Split(':'); //string[]
            string hh;
            string mm;

            if (Convert.ToInt32(times[0]) < 10 && times[0].Length == 1) hh = "0" + times[0];
            else hh = times[0];

            if (Convert.ToInt32(times[1]) < 10 && times[1].Length == 1) mm = "0" + times[1];
            else mm = times[1];

            return hh + mm + "1";
        }
    }
}