using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EMS.Core
{
    public class Common
    {
        public static string GetHourMinute(TimeSpan span)
        {
            var hours = (int)span.TotalHours;
            var minutes = span.Minutes.ToString("00");
            //return string.Format("{0} hr {1} min", hours, minutes);
            string hour = hours > 0 ? hours.ToString() + " hr " : string.Empty;
            string minute = minutes != "00" ? minutes + " min" : string.Empty;
            return hour + minute;
        }
        public static DateTime GetStartDateOfWeek(DateTime value)
        {
            // Get rid of the time part first...
            value = value.Date;
            int daysIntoWeek = (int)value.DayOfWeek;
            DateTime weekStartDate = value.AddDays(-daysIntoWeek);
            return weekStartDate;
        }
        public static DateTime GetEndDateOfWeek(DateTime value)
        {
            // Get rid of the time part last...
            value = value.Date;
            int daysIntoWeek = (int)value.DayOfWeek;
            DateTime weekStartDate = value.AddDays(-daysIntoWeek);
            DateTime weekEndDate = value.AddDays(7 - daysIntoWeek - 1);
            return weekEndDate;
        }
        /// <summary>
        /// gets start date of week
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="culture">Culture object having culture information</param>
        /// <returns>start date of week</returns>
        public static DateTime GetStartDateOfWeek(DateTime date, CultureInfo culture)
        {
            var diff = date.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }


            return date.AddDays(-diff).Date;
        }
        /// <summary>
        /// gets end date of week
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="culture">Culture object having culture information</param>
        /// <returns>end date of week</returns>
        public static DateTime GetEndDateOfWeek(DateTime date, CultureInfo culture)
        {
            return Common.GetStartDateOfWeek(date,culture).AddDays(6);
        }
        /// <summary>
        /// Gets week no. of month For examplev 17 Nov 19 falls in 3rd week in "en-UK" culture
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="culture">Culture object having culture information</param>
        /// <returns>week no</returns>
        public static int GetWeekOfMonth(DateTime date, CultureInfo culture)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != culture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }
        /// <summary>
        /// gets first date of month
        /// </summary>
        /// <param name="dt">Date</param>
        /// <returns>first date of month</returns>
        public static DateTime FirstDayOfMonth(DateTime dt) =>
        new DateTime(dt.Year, dt.Month, 1);
        /// <summary>
        /// gets last day of month
        /// </summary>
        /// <param name="dt">date</param>
        /// <returns>Last day of month</returns>
        public static DateTime LastDayOfMonth(DateTime dt) =>
            FirstDayOfMonth(dt).AddMonths(1).AddDays(-1);

        public static string GetStartEndDateOfWeek(DateTime value)
        {
            // Get rid of the time part first and last date string...
            value = value.Date;
            int daysIntoWeek = (int)value.DayOfWeek;
            DateTime weekStartDate = value.AddDays(-daysIntoWeek);
            DateTime weekEndDate = value.AddDays(7 - daysIntoWeek - 1);
            return weekStartDate.ToString("dd/MM/yyyy") + " - " + weekEndDate.ToString("dd/MM/yyyy");
        }

        public static string GetEMSProjectStatus(string CRMProjectStatus)
        {
            string status = string.Empty;
            switch (CRMProjectStatus)
            {
                case "running":
                case "runing":
                    status = "R";
                    break;
                case "over run":
                case "over-run":
                    status = "O";
                    break;
                case "on hold":
                case "hold":
                    status = "H";
                    break;
                case "complete":
                case "completed":
                    status = "C";
                    break;
                case "remove":
                case "deactive":
                    status = "D";
                    break;
                case "not initiated":
                    status = "I";
                    break;
                case "not converted":
                    status = "N";
                    break;
            }

            return status;
        }

        // Return project status value according to APICRMStatus Enum value
        public static string GetEMSProjectStatusFromAPICRMEnum(string Status)
        {
            string status = string.Empty;
            switch (Status)
            {
                case "1":
                    status = "R";
                    break;
                case "2":
                    status = "H";
                    break;
                case "3":
                case "9":
                    status = "C";
                    break;
            }

            return status;
        }

        // Return project status value according to CRMStatus Enum value
        public static string GetEMSProjectStatusFromCRMStatusEnum(string Status)
        {
            string status = string.Empty;
            switch (Status)
            {              
                case "1":
                    status = "R";
                    break;
                case "2":
                    status = "H";
                    break;
                case "3":
                case "9":
                    status = "C";
                    break;
            }

            return status;
        }

        public static string GetBillingTeam(string BillingTeam)
        {
            string BillTeam = string.Empty;
            switch (BillingTeam)
            {

                case "aus":
                    BillTeam = "AUS";
                    break;
                case "uk":
                    BillTeam = "UK";
                    break;
                case "us":
                    BillTeam = "USA";
                    break;
                case "australia":
                    BillTeam = "AUS";
                    break;
                case "india fl":
                    BillTeam = "INRFL";
                    break;
                case "india":
                    BillTeam = "INR";
                    break;

            }

            return BillTeam;
        }

        public static string GetProjectDisplayStatus(string projectStatus)
        {
            string status = projectStatus;
            projectStatus = !string.IsNullOrWhiteSpace(projectStatus) ? projectStatus.ToLower().Trim() : "";
            switch (projectStatus)
            {
                case "running":
                case "runing":
                case "r":
                    status = "Running";
                    break;
                case "over run":
                case "over-run":
                case "o":
                    status = "Over Run";
                    break;
                case "h":
                case "on hold":
                    status = "Hold";
                    break;
                case "complete":
                case "completed":
                case "c":
                    status = "Completed";
                    break;
                case "remove":
                case "d":
                    status = "Deactive";
                    break;
                case "not initiated":
                case "i":
                    status = "Not Initiated";
                    break;
                case "not converted":
                case "n":
                    status = "Not Converted";
                    break;
            }

            return status;
        }

        public static string GetBillingDisplayTeam(string billingTeam)
        {

            string BillTeam = billingTeam;
            billingTeam = !string.IsNullOrWhiteSpace(billingTeam) ? billingTeam.ToLower().Trim() : "";

            switch (billingTeam)
            {
                case "aus":
                case "Australia":
                    BillTeam = "AUS";
                    break;
                case "uk":
                    BillTeam = "UK";
                    break;
                case "us":
                case "usa":
                    BillTeam = "USA";
                    break;
                case "australia":
                    BillTeam = "AUS";
                    break;
                case "india fl":
                    BillTeam = "INRFL";
                    break;
                case "india":
                    BillTeam = "INR";
                    break;

            }

            return BillTeam;
        }

        public static int[] GetExcludedBucketModelIds()
        {
            return new int[] { (int)Enums.BucketModel.BucketSystemDeveloperOrDesigner, (int)Enums.BucketModel.ServerNetworkEngineerSupport, (int)Enums.BucketModel.WebHosting };
        }

        public static int[] GetSupportTeamRoleIds()
        {
            //return new int[] { (int)Enums.UserRoles.TL, (int)Enums.UserRoles.QA, (int)Enums.UserRoles.BA, (int)Enums.UserRoles.DG };
            return RoleValidator.SupportTeam_RoleIds;
        }
        /// <summary>
        /// Method for HTML stripping
        /// </summary>
        /// <param name="source">source string to be html stripped</param>
        /// <returns></returns>
        public static string StripHTMLTagsRegex(string source)
        {
            return source.HasValue() ? Regex.Replace(source, "<.*?>", string.Empty) : null;
        }

        /// <summary>
        /// Method for iterate on giving date including both from and thru.   
        /// </summary>
        /// <param name="from">From Date</param>
        /// <param name="thru">To Date</param>
        /// <returns></returns>
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static string[] OtherExtensions
        {
            get
            {
                return new string[] { ".pdf", ".docx", ".doc", ".ppt", ".xlsx", ".xls", ".doc", ".csv", ".psd", ".html", ".rar", ".zip" };
            }
        }
        public static string[] ImageExtensions
        {
            get
            {
                return new string[] { ".jpg", ".png", ".jpeg", ".gif" };
            }
        }


    }
}
