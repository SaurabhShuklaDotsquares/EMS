﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace EMS.Web.LIBS
{
    public static class Extension
    {
        public static string Time(this double hours)
        {
            if (hours <= 0)
                return "-";

            var ts = TimeSpan.FromHours(hours);
            return String.Format("{0}:{1}", ts.Hours.ToString("00"), ts.Minutes.ToString("00"));
        }

        public static string TotalLeaveDays(this int LeaveDays)
        {
            if (LeaveDays == 0)
                return "-";
            else if (LeaveDays < 0)
                return "+ (" + Math.Abs(LeaveDays) + ")";
            return LeaveDays.ToString();
        }

        public static string TotalLeaveDays(this decimal LeaveDays)
        {
            if (LeaveDays == 0.0M)
                return "-";
            else if (LeaveDays < 0)
                return "+ (" + Math.Abs(LeaveDays) + ")";
            return LeaveDays.ToString();
        }

        public static string TrimLength(this string str, int length)
        {
            if (!String.IsNullOrEmpty(str))
                return str.Length <= length ? str : str.Substring(0, length) + "...";

            return String.Empty;
        }

        public static bool CompareDate(this DateTime? dateTime, DateTime? otherDate)
        {
            if (dateTime.HasValue && otherDate.HasValue)
            {
                return dateTime.Value.Day == otherDate.Value.Day
                       && dateTime.Value.Month == otherDate.Value.Month
                       && dateTime.Value.Year == otherDate.Value.Year;
            }

            return false;
        }

        //public static string[] SelectedValues(this CheckBoxList CHK)
        //{
        //    List<string> selectedValues = new List<string>();

        //    foreach (ListItem item in CHK.Items)
        //        if (item.Selected)
        //            selectedValues.Add(item.Value);

        //    return selectedValues.ToArray();
        //}

        //public static void SelectedValues(this CheckBoxList CHK, string[] values)
        //{
        //    foreach (ListItem item in CHK.Items)
        //        if (values.Contains(item.Value))
        //            item.Selected = true;
        //}

        public static bool ContainsAny(this string input, params string[] values)
        {
            return String.IsNullOrEmpty(input) ? false : values.Any(S => input.Contains(S));
        }

        //public static void AddOrReplace(this IDictionary<string, object> DICT, string key, object value)
        //{
        //    if (DICT.ContainsKey(key))
        //        DICT[key] = value;
        //    else
        //        DICT.Add(key, value);
        //}

        public static dynamic GetObjectOrDefault(this Dictionary<string, object> DICT, string key)
        {
            if (DICT.ContainsKey(key))
                return DICT[key];
            else
                return null;
        }

        public static T GetObjectOrDefault<T>(this Dictionary<string, object> DICT, string key)
        {
            if (DICT.ContainsKey(key))
                return (T)Convert.ChangeType(DICT[key], typeof(T));
            else
                return default(T);
        }

        public static string ToTitle(this string input)
        {
            return String.IsNullOrEmpty(input) ? String.Empty : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public static void ExportToExcel(this System.Data.DataTable Tbl, string ExcelFilePath = null)
        {
            //try
            //{
            //    if (Tbl == null || Tbl.Columns.Count == 0)
            //        throw new Exception("ExportToExcel: Null or empty input table!\n");

            //    // load excel, and create a new workbook
            //    Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            //    excelApp.Workbooks.Add();

            //    // single worksheet
            //    Microsoft.Office.Interop.Excel._Worksheet workSheet = excelApp.ActiveSheet;

            //    // column headings
            //    for (int i = 0; i < Tbl.Columns.Count; i++)
            //    {
            //        workSheet.Cells[1, (i + 1)] = Tbl.Columns[i].ColumnName;
            //    }

            //    // rows
            //    for (int i = 0; i < Tbl.Rows.Count; i++)
            //    {
            //        // to do: format datetime values before printing
            //        for (int j = 0; j < Tbl.Columns.Count; j++)
            //        {
            //            workSheet.Cells[(i + 2), (j + 1)] = Tbl.Rows[i][j];
            //        }
            //    }

            //    // check fielpath
            //    if (ExcelFilePath != null && ExcelFilePath != "")
            //    {
            //        try
            //        {
            //            workSheet.SaveAs(ExcelFilePath);
            //            excelApp.Quit();

            //        }
            //        catch (Exception ex)
            //        {
            //            throw new Exception("ExportToExcel: Excel file could not be saved! Check filepath.\n"
            //                + ex.Message);
            //        }
            //    }
            //    else    // no filepath is given
            //    {
            //        excelApp.Visible = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("ExportToExcel: \n" + ex.Message);
            //}
        }

        public static string StripHTML(this string HTMLText)
        {
            string buf;
            string block = "address|article|aside|blockquote|canvas|dd|div|dl|dt|" +
              "fieldset|figcaption|figure|footer|form|h\\d|header|hr|li|main|nav|" +
              "noscript|ol|output|p|pre|section|table|tfoot|ul|video";

            string patNestedBlock = $"(\\s*?</?({block})[^>]*?>)+\\s*";
            buf = Regex.Replace(HTMLText, patNestedBlock, "\n", RegexOptions.IgnoreCase);

            // Replace br tag to newline.
            buf = Regex.Replace(buf, @"<(br)[^>]*>", "\n", RegexOptions.IgnoreCase);

            // (Optional) remove styles and scripts.
            buf = Regex.Replace(buf, @"<(script|style)[^>]*?>.*?</\1>", "", RegexOptions.Singleline);

            // Remove all tags.
            buf = Regex.Replace(buf, @"<[^>]*(>|$)", "", RegexOptions.Multiline);

            // Replace HTML entities.
            buf = WebUtility.HtmlDecode(buf);
            return buf;
        }

        public static DateTime ToDateTimes(this string str, bool isWithTime = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str))
                    return DateTime.Now;

                if (isWithTime)
                {
                    if (DateTime.TryParseExact(str, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        return result;
                    }
                    else if (DateTime.TryParseExact(str, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    {
                        return result;
                    }
                }
            }
            catch
            {
                return DateTime.ParseExact(str, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            return DateTime.ParseExact(str, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

    }
}