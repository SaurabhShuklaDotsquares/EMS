using EMS.Dto;
using EMS.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Code.LIBS
{
    public static class EMSHttpContext
    {
        private static IServiceProvider services = null;

        /// <summary>
        /// Provides static access to the framework's services provider
        /// </summary>
        public static IServiceProvider Services 
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }
    }

    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class WebsiteSession
    {
        public static Dictionary<string, object> SessionProjectStatus
        {
            get
            {
                ISession session = ContextProvider.HttpContext.Session;

                if (session.GetObjectFromJson<Dictionary<string, object>>(nameof(SessionProjectStatus)) == null)
                    session.SetObjectAsJson(nameof(SessionProjectStatus), new Dictionary<string, object>());

                return session.GetObjectFromJson<Dictionary<string, object>>(nameof(SessionProjectStatus));
            }
        }

        public static void UpdateSessionProjectStatus(Dictionary<string, object> data)
        {
            if (data == null)
                data = new Dictionary<string, object>();

            ContextProvider.HttpContext.Session.SetObjectAsJson(nameof(SessionProjectStatus), data);
        }

        public static void RemoveProjectStatusSession()
        {
            ContextProvider.HttpContext.Session.Remove("SessionProjectStatus");
        }
    }
}