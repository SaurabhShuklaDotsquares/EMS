using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Website.Code.LIBS
{
    public static class TempDataExtensions
    {
       
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            
            tempData[key] = JsonConvert.SerializeObject(value);
            tempData.Keep(key);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.Peek(key);
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}
