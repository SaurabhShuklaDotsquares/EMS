using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace EMS.Core
{
   public static class Extensions

    {
        public static string GetEnumDisplayName(this Enum enumType)
        {
            var memberInfo = enumType.GetType().GetMember(enumType.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                return memberInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? enumType.ToString();
            }
            return enumType.ToString();
        }
        public static string GetEnumDisplayShortName(this Enum enumType)
        {
            var memberInfo = enumType.GetType().GetMember(enumType.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                return memberInfo.GetCustomAttribute<DisplayAttribute>()?.ShortName ?? enumType.ToString();
            }
            return enumType.ToString();
        }

        public static void AddOrReplace(this IDictionary<string, object> DICT, string key, object value)
        {
            if (DICT.ContainsKey(key))
                DICT[key] = value;
            else
                DICT.Add(key, value);
        }

        public static dynamic NewGetObjectOrDefault(this Dictionary<string, object> DICT, string key)
        {
            if (DICT != null && DICT.ContainsKey(key))
                return DICT[key];
            else
                return null;
        }

        /// <summary>
        /// Extension method to return an enum value of type T for the given string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Extension method to return an enum value of type T for the given int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int value)
        {
            var name = Enum.GetName(typeof(T), value);
            return name.ToEnum<T>();
        }

        public static IDictionary<string, int> EnumToDictionaryWithDescription(this Type t)
        {
            if (t == null) throw new NullReferenceException();
            if (!t.IsEnum) throw new InvalidCastException("object is not an Enumeration");

            string[] names = Enum.GetNames(t);
            Array values = Enum.GetValues(t);

            return Enumerable.Range(0, names.Length)
                .Select(i => new { Key = ((Enum)values.GetValue(i)).GetDescription(), Value = Convert.ToInt32((Enum)values.GetValue(i)) })
                .ToDictionary(k => k.Key, k => k.Value);
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Name;
            else
                return value.ToString();
        }
        public static string GetEnumDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }

        public static string GetDetailedTimeString(this TimeSpan time)
        {
            StringBuilder timestring = new StringBuilder();
            if (time.Hours > 0)
                timestring.Append(time.Hours + (time.Hours > 1 ? " Hours " : " Hour "));
            if (time.Minutes > 0)
            {
                timestring.Append(time.Minutes + (time.Minutes > 1 ? " Minutes" : " Minute"));
            }
            return timestring.ToString();
        }

        public static string GetTimeHoursFormatString(this double timeHours)
        {
            var s = string.Format("{0:0.00}", timeHours);
            if (s.EndsWith("00"))
            {
                return ((int)timeHours).ToString();
            }
            else
            {
                return s;
            }
        }

        public static string DateTimeConversion(this double minutes)
        {
            string totalHours = string.Empty;
            string totalMinutes = string.Empty;
            if (minutes > 60)
            {
                if (Convert.ToInt32(minutes / 60) > 1)
                {
                    totalHours = Convert.ToInt32(minutes / 60).ToString() + " Hours";
                }
                else
                {
                    totalHours = Convert.ToInt32(minutes / 60).ToString() + " Hour";
                }
                if (Convert.ToInt32(minutes % 60) > 1)
                {
                    totalMinutes = Convert.ToInt32(minutes % 60).ToString() + " Minutes";
                }
                else if (Convert.ToInt32(minutes % 60) == 1)
                {
                    totalMinutes = Convert.ToInt32(minutes % 60).ToString() + " Minute";
                }

            }
            return $"{totalHours} {totalMinutes}";
        }

        public static string TrimLength(this string input, int length, bool incomplete = true)
        {
            if (String.IsNullOrEmpty(input))
            {
                return String.Empty;
            }
            else
            {
                return input.Length > length ? String.Concat(input.Substring(0, length), (incomplete ? "..." : "")) : input;
            }
        }

        public static string ToCombinedString(this string[] inputStringArray, string delimiter = ", ")
        {
            if (inputStringArray == null)
            {
                return null;
            }

            if (!inputStringArray.Any())
            {
                return string.Empty;
            }
          
            return string.Join(delimiter, inputStringArray);
        }

        public static T[] ToArray<T>(this string combinedString, string delimiterUsed = ",")
        {
            if (combinedString.HasValue())
            {
                return combinedString.Trim().Split(new string[] { delimiterUsed }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(x => (T)Convert.ChangeType(x.Trim(), typeof(T))).ToArray();
            }
            return null;
        }

        #region DateTime Formats..
        //public static string ToFormatDateString(this DateTime? date, string format = "MM/dd/yyyy")
        //{
        //    try
        //    {
        //        if (!date.HasValue)
        //            return string.Empty;
        //        return date.Value.ToString(format);
        //    }
        //    catch (Exception ex)
        //    {
        //        return string.Empty;
        //    }

        //}

        public static string ToDateWithMonthString(this DateTime date)
        {
            try
            {

                return date.ToString("dd MMM yyyy");
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        public static string ToDateWithTimeString(this DateTime? date)
        {
            try
            {
                if (!date.HasValue)
                    return string.Empty;
                return date.Value.ToString("dd MMM yyyy hh:mm tt");
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        //public static string ToFormatDateString(this DateTime date, string format = "dd/MM/yyyy")
        //{
        //    try
        //    {
        //        return date.ToString(format);
        //    }
        //    catch (Exception ex)
        //    {
        //        return string.Empty;
        //    }

        //}

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        public static string ToFormatDateString(this DateTime date, string format = "MMM d yyyy")
        {
            string retVal = "";
            try
            {
                retVal = date.ToString(format, CultureInfo.InvariantCulture);
            }
            catch { }
            return retVal;
        }

        public static string ToFormatDateString(this DateTime? date, string format = "MMM d yyyy")
        {
            if (date.HasValue)
            {
                return date.Value.ToFormatDateString(format);
            }
            return null;
        }

        public static DateTime? ToDateTime(this string str, bool isWithTime = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return (DateTime?)null;

            string[] formats = { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "dd/MM/yyyy h:mm:ss tt", "d/MM/yyyy h:mm:ss tt", "dd/M/yyyy h:mm:ss tt", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d", "yyyy-MM-dd h:mm:ss tt", "yyyy-M-dd h:mm:ss tt", "yyyy-MM-d h:mm:ss tt", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "dd-MM-yyyy h:mm:ss tt", "d-MM-yyyy h:mm:ss tt", "dd-M-yyyy h:mm:ss tt", "yyyy/MM/dd", "yyyy/M/dd", "yyyy/MM/d", "yyyy/MM/dd  h:mm:ss tt", "yyyy/M/dd  h:mm:ss tt", "yyyy/MM/d  h:mm:ss tt", "d/M/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "MM/d/yyyy h:mm:ss tt", "M/dd/yyyy", "MM/dd/yyyy", "MM/d/yyyy", "yyyyMMdd", "d/M/yy" };
            //CultureInfo enGB = new CultureInfo("en-GB");

            if (isWithTime)
            {
                return DateTime.ParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            return DateTime.ParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public static DateTime? ToDateTime(this string inputDateTime, string inputDateTimeFormat)
        {
            if (string.IsNullOrWhiteSpace(inputDateTime) || string.IsNullOrWhiteSpace(inputDateTimeFormat))
            {
                return null;
            }
            try
            {
                return DateTime.ParseExact(inputDateTime.Trim(), inputDateTimeFormat.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch { return null; }
        }

        public static DateTime? ToDateTimeDDMMYYYY(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return (DateTime?)null;

            string[] formats = { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "dd/MM/yyyy h:mm:ss tt", "d/MM/yyyy h:mm:ss tt", "dd/M/yyyy h:mm:ss tt", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "dd-MM-yyyy h:mm:ss tt", "d-MM-yyyy h:mm:ss tt", "dd-M-yyyy h:mm:ss tt", "d/M/yyyy h:mm:ss tt", "d/M/yy", "d-M-yy" };
            return DateTime.ParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public static string NewLineToHtmlBreak(this string inputString)
        {
            if (!string.IsNullOrWhiteSpace(inputString))
            {
                return inputString.Trim().Replace("\n", "<br>").Replace(Environment.NewLine, "<br>").Replace("\r\n", "<br>");
            }
            return inputString;
        }

        #endregion

        public static int? ToInt32(this string inputValue)
        {
            if (!string.IsNullOrWhiteSpace(inputValue))
            {
                int intValue;
                if (int.TryParse(inputValue, out intValue))
                {
                    return intValue;
                }
            }
            return null;
        }

        public static string ToSelfURL(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            string outputStr = text.Trim().Replace(":", "-").Replace("&", "-").Replace(" ", "-").Replace("'", "-").Replace(",", "-").Replace("(", "-").Replace(")", "-").Replace("--", "-").Replace(".", "-");
            return Regex.Replace(outputStr.Trim().ToLower().Replace("--", ""), "[^a-zA-Z0-9_-]+", "-", RegexOptions.Compiled).Replace("--", "-");
        }

        public static bool HasValue(this string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        public static string ToTitleCase(this string input)
        {
            return input.HasValue() ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input) : input;
        }

        public static IEnumerable<DateTime> Range(this DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, (endDate - startDate).Days + 1).Select(d => startDate.AddDays(d));
        }
        /// <summary>
        /// gets short month name
        /// </summary>
        /// <param name="dateTime">date</param>
        /// <param name="culture">culture object having culture information</param>
        /// <returns>Returns abbreviated month name i.e. Nov for November</returns>
        public static string ToShortMonthName(this DateTime dateTime, CultureInfo culture)
        {
            return culture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }

        public static string ToUnique(this string fileName)
        {
            return $"{DateTime.Now.Ticks}{Path.GetExtension(fileName.ToLower())}";
        }
    }
}
