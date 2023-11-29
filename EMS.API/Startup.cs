using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.API.DAL;
using EMS.API.LIBS;
using EMS.Data;
using EMS.Data.saral;
using EMS.Data.saralDT;
using EMS.Repo;
using EMS.Service;
using EMS.Service.LibraryManagement;
using EMS.Service.SARAL;
using EMS.Service.SARALDT;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMS.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc();/*.AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); */
            services.AddSingleton(_ => Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddDbContext<db_dsmanagementnewContext>(options => options.UseSqlServer(Configuration.GetConnectionString("dsmanagementConnection"), builder => builder.UseRowNumberForPaging()));
            services.AddDbContext<db_saralContext>(options => options.UseSqlServer(Configuration.GetConnectionString("saralConnection"), builder => builder.UseRowNumberForPaging()));
            services.AddDbContext<db_saralDTContext>(options => options.UseSqlServer(Configuration.GetConnectionString("saralConnectionDT"), builder => builder.UseRowNumberForPaging()));
            SiteKeys.Configure(Configuration);
            InitServices(services);
        }

        public void InitServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserLoginService, UserLoginService>();
            services.AddScoped<IApiKeyService, ApiKeyService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ITechnologyService, TechnologyService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITimesheetService, TimesheetService>();
            services.AddScoped<IVirtualDeveloperService, VirtualDeveloperService>();
            services.AddScoped<IProjectClosureService, ProjectClosureService>();
            services.AddScoped<IBucketModelService, BucketModelService>();
            services.AddScoped<ICRMData, CRMData>();
            services.AddScoped<IManageLog, ManageLog>();
            services.AddScoped<IProjectClientFeedbackService, ProjectClientFeedbackService>();
            services.AddScoped<IEscalationService, EscalationService>();
            services.AddScoped<IComplaintService, ComplaintService>();
            services.AddScoped<ITechnologyParentMappingService, TechnologyParentMappingService>();
            services.AddScoped<ITechnologyParentService, TechnologyParentService>();
            services.AddScoped<ILibraryManagementService, LibraryManagementService>();
            services.AddScoped<IDomainTypeService, DomainTypeService>();
            services.AddScoped<ILibrarySearchService, LibrarySearchService>();
            services.AddScoped<ILibraryDownloadService, LibraryDownloadService>();
            services.AddScoped<ILibraryDownloadHistoryService, LibraryDownloadHistoryService>();
            services.AddScoped<ILibraryTemplateTypeService, LibraryTemplateTypeService>();
            services.AddScoped<ILibraryLayoutService, LibraryLayoutService>();
            services.AddScoped<ILibraryComponentTypeService, LibraryComponentTypeService>();
            services.AddScoped<IUserActivityService, UserActivityService>();
            services.AddScoped<ILeaveService, LeaveService>();
            services.AddScoped<ILevDetailsService, LevDetailsService>();
            services.AddScoped<ILevDetailsDTService, LevDetailsDTService>();
            services.AddScoped<IWFHService, WFHService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IKRAServices, KRAServices>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }
    }
}
