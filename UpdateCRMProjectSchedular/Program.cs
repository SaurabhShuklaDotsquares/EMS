using EMS.Data;
using EMS.Repo;
using EMS.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using UpdateCRMProjectSchedular.DAL;

namespace UpdateCRMProjectSchedular
{
    class Program
    {
        public static IConfigurationRoot configuration;      
        private static IUserLoginService userLoginService;       
        static void Main(string[] args)
        {
            //Console.WriteLine("----- Start Scheduler ---------------");
            ConfigureServices();
            //ManageLog.WriteLogFile("============================================ Start Scheduler =====================================================================");
            //Console.WriteLine("----- Configured Services ---------------");
            UpdateCRM();
            //Console.WriteLine("----- End Scheduler ---------------");
            //System.Threading.Thread.Sleep(10000);
        }
        public static void ConfigureServices()
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
            services.AddScoped<IUserLoginService, UserLoginService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IVirtualDeveloperService, VirtualDeveloperService>();
            services.AddScoped<IBucketModelService, BucketModelService>();
            services.AddScoped<ITechnologyService, TechnologyService>();

            var serviceProvider = services.BuildServiceProvider();
            userLoginService = serviceProvider.GetService<IUserLoginService>();            
            CRMData.RegisterService(serviceProvider);      
            
            SiteKeys.Configure(configuration);
        }     

        private static void UpdateCRM()
        {
            ManageLog.WriteLogFile("=================================================================================================================");
            ManageLog.WriteLogFile("============================== Schedular Started (" + DateTime.Now.ToString() + ") ==============================");
            ManageLog.WriteLogFile("=================================================================================================================");
            try
            {
                var pmList = userLoginService.GetUsersWithApidata();
                if (!SiteKeys.IsLive)
                {
                    pmList = pmList.Where(p => p.Uid == 2).ToList();
                }
                foreach (var pm in pmList)
                {
                    ManageLog.WriteLogFile("============================== Project Manager(" + pm.Uid + " - " + pm.Name + ") Started (" + DateTime.Now.ToString() + ") ==============================");

                    CRMData.UpdateCRMData(pm.Uid, pm, pm.IsSuperAdmin ?? false);

                    ManageLog.WriteLogFile("============================== Project Manager(" + pm.Uid + " - " + pm.Name + ") Completed (" + DateTime.Now.ToString() + ") ==============================");
                }
            }
            catch (Exception ex)
            {
                ManageLog.WriteLogFile("============================== Schedular Error (" + ex.Message.ToString() + ") ==============================");
            }
            ManageLog.WriteLogFile("===================================================================================================================");
            ManageLog.WriteLogFile("============================== Schedular Completed (" + DateTime.Now.ToString() + ") ==============================");
            ManageLog.WriteLogFile("===================================================================================================================");
            ManageLog.WriteLogFile("\n");
        }       

    }
}
