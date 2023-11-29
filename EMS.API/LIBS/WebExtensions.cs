using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.LIBS
{
    public static class WebExtensions
    {
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
    }
}
