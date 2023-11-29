using EMS.Data;
using EMS.Repo;
using EMS.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LibraryUpdateNotificationScheduler
{
    public class Program
    {
        public static IConfigurationRoot configuration;
        private static ILibraryManagementService libraryManagementService;
        private static ITechnologyParentService technologyParentService;
        private static ITechnologyParentMappingService technologyParentMappingService;
        private static IUserLoginService userLoginService;
        public static void Main(string[] args)
        {
            ConfigureServices();
            TechnologyOwnerNotification();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
            services.AddSingleton<IConfigurationRoot>(configuration);

            var connection = configuration["ConnectionStrings:dsmanagementConnection"];

            services.AddDbContext<db_dsmanagementnewContext>(options => options.UseSqlServer(connection));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITechnologyParentService, TechnologyParentService>();
            services.AddScoped<ILibraryManagementService, LibraryManagementService>();
            services.AddScoped<ITechnologyParentMappingService, TechnologyParentMappingService>();
            services.AddScoped<IUserLoginService, UserLoginService>();

            var serviceProvider = services.BuildServiceProvider();
            libraryManagementService = serviceProvider.GetService<ILibraryManagementService>();
            technologyParentService = serviceProvider.GetService<ITechnologyParentService>();
            technologyParentMappingService = serviceProvider.GetService<ITechnologyParentMappingService>();
            userLoginService = serviceProvider.GetService<IUserLoginService>();

            SiteKeys.Configure(configuration);
        }

        private static void TechnologyOwnerNotification()
        {
            try
            {
                var RecentlyAddedTechnology = libraryManagementService.GetUnModifiedLibraryTechnologyParents(SiteKeys.AlertDays);

                var ParentTechnologies = technologyParentService.GetTechnologyParentList();
                List<int> RecentlyAddedTechnologyList = new List<int>();
                List<int> ParentTechnologyList = new List<int>();
                if (RecentlyAddedTechnology != null)
                    RecentlyAddedTechnologyList = RecentlyAddedTechnology.Select(x => x.Id).ToList();
                if (ParentTechnologies != null)
                    ParentTechnologyList = ParentTechnologies.Select(y => y.Id).ToList();
                var OwnerEmailList = ParentTechnologies.Except(RecentlyAddedTechnology).ToList();
                if (OwnerEmailList.Count > 0)
                {
                    foreach (var owner in OwnerEmailList)
                    {
                        string days = SiteKeys.AlertDays.ToString();
                        var TechnologyParentList = libraryManagementService.GetLibraryDate(owner.Id);
                        //if(LibraryList.TechnologyParentMapping.Select(x => x.Technology.LibraryTechnology.()).ToList())
                        var LibraryList = TechnologyParentList.TechnologyParentMapping.Select(x => x.Technology.LibraryTechnology.Select(y => y.Library)).ToList();
                        foreach (var item in LibraryList)
                        {
                            var LibraryDate = item.ToList().Where(x => x.AddDate < DateTime.Now.AddDays(-SiteKeys.AlertDays)).OrderByDescending(x => x.AddDate).FirstOrDefault();
                            if (LibraryDate != null)
                            {
                                if ((DateTime.Now - LibraryDate.AddDate).HasValue)
                                    days = Math.Round((DateTime.Now - LibraryDate.AddDate).Value.TotalDays).ToString();
                            }
                            else
                            {
                                var firstLibrary = libraryManagementService.GetFirstLibrary();
                                if (firstLibrary != null)
                                    if ((DateTime.Now - firstLibrary.AddDate).HasValue)
                                        days = Math.Round((DateTime.Now - firstLibrary.AddDate).Value.TotalDays).ToString();
                            }
                        }

                        var Email = owner.Email.Trim();
                        var EmailList = Email.Split(',');
                        if (EmailList.Length > 0)
                        {
                            foreach (var email in EmailList)
                            {
                                var users = userLoginService.GetUsers().Where(x => x.EmailOffice == email).FirstOrDefault();
                                if (users != null)
                                {
                                    FlexiMail mailer = new FlexiMail();
                                    mailer.ValueArray = new string[]
                                    {
                                        users.Name,
                                        days
                                    };
                                    mailer.Subject = $"{owner.Title}: No updates last {days} days";
                                    mailer.MailBodyManualSupply = true;
                                    mailer.MailBody = mailer.GetHtml($"{GetApplicationRoot()}\\EmailTemplate\\LibraryUpdateNotification.html");
                                    mailer.To = users.EmailOffice;
                                    mailer.CC = SiteKeys.EmailCC;
                                    mailer.BCC = SiteKeys.EmailBCC;
                                    mailer.From = "no-reply@dotsquares.com";
                                    mailer.Send();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
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
