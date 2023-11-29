using EMS.Dto;
using NeoSmart.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using EMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList.Core;

namespace EMS.Web.Code.LIBS
{
    public static class WebExtensions
    {
        public static List<DropdownListDto> GetList<TEnum>(bool getDisplayName = false) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new InvalidOperationException();

            var enumList = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            return enumList.Select(x => new DropdownListDto()
            {
                Text = getDisplayName ? x.GetEnumDisplayName() : x.ToString(),
                Value = Convert.ToInt32(x).ToString(),
                Id = Convert.ToInt32(x)
            }).ToList();
        }

        private static string GetEnumDisplayName<T>(this T value) where T : struct
        {
            var type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(value)} must be of Enum type", nameof(value));
            }

            var memberInfo = type.GetMember(value.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                var attr = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

                if (attr != null)
                {
                    return ((DisplayAttribute)attr).Name;
                }
            }

            return value.ToString();
        }

        public static string ToUrlBase64String(this string value)
        {
            if (value.HasValue())
            {
                return UrlBase64.Encode(Encoding.UTF8.GetBytes(value));
            }
            return null;
        }

        public static string FromUrlBase64ToString(this string value)
        {
            if (value.HasValue())
            {
                return Encoding.UTF8.GetString(UrlBase64.Decode(value));
            }
            return null;
        }

        public static string ToUrlBase64String(this int value)
        {
            return value.ToString().ToUrlBase64String();
        }

        public static Int32? UrlBaseToInt32(this string value)
        {
            if (value.HasValue())
            {
                return Encoding.UTF8.GetString(UrlBase64.Decode(value)).ToInt32();
            }
            return null;
        }

        public static List<SelectListItem> GetSelectList<TEnum>(params TEnum[] ignoreEnums) where TEnum : struct
        {
            var enumList = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            if (ignoreEnums != null)
            {
                enumList = enumList.Except(ignoreEnums);
            }

            return enumList.Select(v => new SelectListItem
            {
                Text = v.GetEnumDisplayName(),
                Value = Convert.ToInt32(v).ToString()
            }).ToList();
        }

        public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> collection, Func<T, string> nameGetter, Func<T, int> idGetter) where T : class
        {
            return collection.Select(m => new SelectListItem
            {
                Text = nameGetter(m),
                Value = idGetter(m).ToString()
            }).ToList();
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageNumber, int pageSize) where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException($"source list cannot be null");
            }

            return new StaticPagedList<T>(
                         source.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                         pageNumber, pageSize,
                         source.Count());
        }
    }
}