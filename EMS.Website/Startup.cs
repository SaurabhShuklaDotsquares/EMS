using DataTables.AspNet.AspNetCore;
using EMS.Data;
using EMS.Data.saral;
using EMS.Data.saralDT;
using EMS.Dto;
using EMS.Dto.LibraryManagement;
using EMS.Repo;
using EMS.Service;
using EMS.Service.SARAL;
using EMS.Service.SARALDT;
using EMS.Service.LibraryManagement;
using EMS.Validations;
using EMS.Web.Code.LIBS;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;
using EMS.Service.CVEstimatePrice;
using EMS.Dto.CVEstimatePrice;
using EMS.Validations.StudyDocuments;
using EMS.Validations.Sme;

namespace EMS.Website
{

    public partial class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // public static IHttpContextAccessor HttpContextAccessor;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // "Multipart body length limit 134217728 exceeded" to prevent this issue
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            services.AddSingleton(_ => Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddDbContext<db_dsmanagementnewContext>(options => options.UseSqlServer(Configuration.GetConnectionString("dsmanagementConnection"), builder => builder.UseRowNumberForPaging()));
            services.AddDbContext<db_saralContext>(options => options.UseSqlServer(Configuration.GetConnectionString("saralConnection"), builder => builder.UseRowNumberForPaging()));
            services.AddDbContext<db_saralDTContext>(options => options.UseSqlServer(Configuration.GetConnectionString("saralConnectionDT"), builder => builder.UseRowNumberForPaging()));


            services.AddMvc().AddFluentValidation();

            SiteKey.Configure(Configuration.GetSection("SiteKeys"));

            services.RegisterDataTables();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
            {
                opt.LoginPath = "/Login";
            });

            services.Configure<FormOptions>(x => x.ValueCountLimit = 1048576);

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                //options.IdleTimeout = TimeSpan.FromSeconds(1000);
                //options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;

            });

            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });

            //services.AddSession();

            AddWebOptimizer(services);
            InitServices(services);
            InitValidation(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                string sHost = context.Request.Host.HasValue == true ? context.Request.Host.Value : "";
                sHost = sHost.ToLower();
                var url = context.Request.GetDisplayUrl();
                string sPath = context.Request.Path.HasValue == true ? context.Request.Path.Value : "";
                string sQuerystring = context.Request.QueryString.HasValue == true ? context.Request.QueryString.Value : "";

                if (url.Contains(SiteKey.DomainNameInternalIP) && SiteKey.IsLive && !url.Contains("192.168") && !url.Contains("ds212.projectstatus.co.uk"))
                {
                    context.Response.Redirect(SiteKey.DomainName);
                }

                if (!context.Request.IsHttps && SiteKey.IsLive && !sHost.Contains("localhost") && !sHost.Contains("192.168") && !sHost.Contains("ds212.projectstatus.co.uk"))
                {
                    //--< is http >--
                    string new_https_Url = "https://" + sHost;
                    if (sPath != "")
                    {
                        new_https_Url = new_https_Url + sPath;
                    }
                    if (sQuerystring != "")
                    {
                        new_https_Url = new_https_Url + sQuerystring;
                    }
                    context.Response.Redirect(new_https_Url);
                    return;
                    //--</ is http >--
                }

                    await next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            EMSHttpContext.Services = app.ApplicationServices;

            app.UseAuthentication();
            app.UseWebOptimizer();

            app.UseSession();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseCookiePolicy();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                }
            });
            RotativaConfiguration.Setup(env);
            ContextProvider.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>(), env);
        }

        /// <summary>
        /// Initializes the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void InitServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAppraiseService, AppraiseService>();
            services.AddScoped<IBucketModelService, BucketModelService>();
            services.AddScoped<IBugReportService, BugReportService>();
            services.AddScoped<ICompanyDocumentService, CompanyDocumentService>();
            services.AddScoped<IComplaintService, ComplaintService>();
            services.AddScoped<IComponentService, ComponentService>();
            services.AddScoped<IComponentCategoryService, ComponentCategoryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<ICurrentOpeningService, CurrentOpeningService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IDeviceDetailService, DeviceDetailService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IForecastingService, ForecastingService>();
            services.AddScoped<IInvestmentService, InvestmentService>();
            services.AddScoped<IProjectInvoiceService, ProjectInvoiceService>();
            services.AddScoped<IJobReferenceService, JobReferenceService>();
            services.AddScoped<ILeadServices, LeadServices>();
            services.AddScoped<ILeadStatusModelService, LeadStatusModelService>();
            services.AddScoped<ILeaveService, LeaveService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IEmployeeMedicalService, EmployeeMedicalService>();
            services.AddScoped<IMeetingRoomService, MeetingRoomService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IOrgDocumentService, OrgDocumentService>();
            services.AddScoped<IPILogService, PILogService>();
            services.AddScoped<IPreferenceService, PreferenceService>();
            services.AddScoped<IProductLandingService, ProductLandingService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectClosureService, ProjectClosureService>();
            services.AddScoped<IProjectClosureReviewService, ProjectClosureReviewService>();
            services.AddScoped<IProjectLessonLearnedService, ProjectLessonLearnedService>();
            services.AddScoped<IProjectNCLogService, ProjectNCLogService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITechnologyService, TechnologyService>();
            services.AddScoped<IDomainTypeService, DomainTypeService>();
            services.AddScoped<ITimesheetService, TimesheetService>();
            services.AddScoped<ITypeMasterService, TypeMasterService>();
            services.AddScoped<IUserActivityService, UserActivityService>();
            services.AddScoped<IUserLoginService, UserLoginService>();
            services.AddScoped<IVirtualDeveloperService, VirtualDeveloperService>();
            services.AddScoped<IMinutesOfMeetingMasterService, MinutesOfMeetingMasterService>();
            services.AddScoped<IMinutesOfMeetingService, MinutesOfMeetingService>();
            services.AddScoped<IMinutesOfMeetingTaskCommentService, MinutesOfMeetingTaskCommentService>();
            services.AddScoped<IMinutesOfMeetingTaskService, MinutesOfMeetingTaskService>();
            services.AddScoped<IProjectClientFeedbackService, ProjectClientFeedbackService>();
            services.AddScoped<IComplaintUserService, ComplaintUserService>();
            services.AddScoped<ILibraryManagementService, LibraryManagementService>();
            services.AddScoped<ILibraryLayoutService, LibraryLayoutService>();
            services.AddScoped<IDomainTypeService, DomainTypeService>();
            services.AddScoped<ILibrarySearchService, LibrarySearchService>();
            services.AddScoped<ILibraryFileTypeService, LibraryFileTypeService>();
            services.AddScoped<ILibraryFileService, LibraryFileService>();
            services.AddScoped<ILibraryComponentFileService, LibraryComponentFileService>();
            services.AddScoped<ILibraryDownloadService, LibraryDownloadService>();
            services.AddScoped<ILibraryDownloadHistoryService, LibraryDownloadHistoryService>();
            services.AddScoped<ILibraryComponentTypeService, LibraryComponentTypeService>();
            services.AddScoped<ITechnologyParentService, TechnologyParentService>();
            services.AddScoped<ITechnologyParentMappingService, TechnologyParentMappingService>();
            services.AddScoped<ISerialNumberService, SerialNumberService>();
            services.AddScoped<IProcessService, ProcessService>();

            services.AddScoped<IOrgImprovementService, OrgImprovementService>();
            services.AddScoped<ISelfDeclarationService, SelfDeclarationService>();
            services.AddScoped<ILibraryTemplateTypeService, LibraryTemplateTypeService>();
            services.AddScoped<ILessonLearntService, LessonLearntService>();
            services.AddScoped<IEstimateHourService, EstimateHourService>();
            services.AddScoped<IEstimatePriceService, EstimatePriceService>();
            services.AddScoped<IEstimateService, EstimateService>();
            services.AddScoped<IEstimateHostingPackageService, EstimateHostingPackageService>();
            services.AddScoped<IEscalationService, EscalationService>();
            services.AddScoped<IEscalationTypeService, EscalationTypeService>();
            services.AddScoped<IEscalationRootCauseService, EscalationRootCauseService>();
            services.AddScoped<IVaccinationService, VaccinationService>();
            services.AddScoped<ILevAllotmentService, LevAllotmentService>();
            services.AddScoped<ILevDetailsService, LevDetailsService>();
            services.AddScoped<ILevMonthdetService, LevMonthdetService>();
            services.AddScoped<ILevAllotmentDTService, LevAllotmentDTService>();
            services.AddScoped<ILevDetailsDTService, LevDetailsDTService>();
            services.AddScoped<ILevMonthdetDTService, LevMonthdetDTService>();
            services.AddScoped<ISalesKitTypeService, SalesKitTypeService>();
            services.AddScoped<ICvsTypeService, CvsTypeService>();
            services.AddScoped<ITeamHierarchyService, TeamHierarchyService>();
            services.AddScoped<IWFHService, WFHService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IKRAServices, KRAServices>();
            services.AddScoped<IEmployeeMonthwiseService, EmployeeMonthwiseService>();
            services.AddScoped<ITDSService, TDSService>();
            services.AddScoped<ICVBuilderService, CVBuilderService>();
            services.AddScoped<ICVEstimatePriceService, CVEstimatePriceService>();
            services.AddScoped<IDocumentLibraryService, DocumentLibraryService>();
            //services.Configure<ForwardedHeadersOptions>(options => {
            //    options.ForwardedHeaders =
            //        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            //});
            services.AddScoped<IStudyDocumentsService, StudyDocumentsService>();
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<ISubjectMasterExpertService, SubjectMasterExpertService>();
        }

        /// <summary>
        /// Initializes the validation.
        /// </summary>
        /// <param name="services">The services.</param>
        public void InitValidation(IServiceCollection services)
        {

            services.AddTransient<IValidator<LoginDto>, LoginDtoValidator>();
            services.AddTransient<IValidator<ForgotPasswordDto>, ForgotPasswordDtoValidator>();
            services.AddTransient<IValidator<UserProfileDto>, UserProfileDtoValidator>();
            services.AddTransient<IValidator<CurrentOpeningDto>, CurrentOpeningDtoValidator>();
            services.AddTransient<IValidator<JobReferFriendDto>, JobReferFriendDtoValidator>();
            services.AddTransient<IValidator<ChangePasswordDto>, ChangePasswordDtoValidator>();
            services.AddTransient<IValidator<DepartmentDto>, DepartmentDtoValidator>();
            services.AddTransient<IValidator<VirtualDeveloperDto>, VirtualDeveloperDtoValidator>();
            services.AddTransient<IValidator<TimeSheetReviewDto>, TimeSheetReviewDtoValidator>();
            services.AddTransient<IValidator<ProjectDto>, ProjectDtoValidator>();
            services.AddTransient<IValidator<LeaveActivityDto>, LeaveActivityDtoValidator>();
            services.AddTransient<IValidator<WFHActivityDto>, WFHActivityDtoValidator>();
            services.AddTransient<IValidator<UploadDocumentDto>, UploadDocumentDtoValidator>();
            services.AddTransient<IValidator<ManageUserDto>, ManageUserDtoValidator>();
            services.AddTransient<IValidator<PreferenceDto>, PreferenceDtoValidator>();
            services.AddTransient<IValidator<ComponentDto>, ComponentDtoValidator>();
            services.AddTransient<IValidator<MedicalDataDto>, MedicalDataDtoValidator>();
            services.AddTransient<IValidator<TaskCommentDto>, TaskCommentDtoValidator>();
            services.AddTransient<IValidator<CreateTaskDto>, CreateTaskDtoValidator>();
            services.AddTransient<IValidator<Commentdto>, CommentDtoValidator>();
            services.AddTransient<IValidator<AddMeetingRoomDto>, MeetingRoomDtoValidator>();
            services.AddTransient<IValidator<ProjectClosureDto>, ProjectClosureDtoValidator>();
            services.AddTransient<IValidator<ProjectClosureStatusDto>, ProjectClosureStatusDtoValidator>();
            services.AddTransient<IValidator<ProjectClousreDetailDto>, ProjectClosureDetailDtoValidator>();
            services.AddTransient<IValidator<ProjectAdditionalSupportDto>, ProjectAdditionalSupportDtoValidator>();
            services.AddTransient<IValidator<InvestmentDto>, InvestmentDtoValidator>();
            services.AddTransient<IValidator<OrgDocumentDto>, OrgDocumentDtoValidator>();
            services.AddTransient<IValidator<ProjectLessonDto>, ProjectLessonDtoValidator>();
            services.AddTransient<IValidator<ProjectLessonLearnedDto>, ProjectLessonLearnedDtoValidator>();
            services.AddTransient<IValidator<ProjectNCLogDto>, ProjectNCLogDtoValidator>();
            services.AddTransient<IValidator<ProjectNCLogAuditeeDto>, ProjectNCLogAuditeeDtoValidator>();
            services.AddTransient<IValidator<PILogRequestDto>, PILogRequestDtoValidator>();
            services.AddTransient<IValidator<PILogApprovalDto>, PILogApprovalDtoValidator>();
            //services.AddTransient<IValidator<MinutesOfMeetingDto>, MinutesOfMeetingDtoValidator>();
            services.AddTransient<IValidator<DeviceDto>, DeviceDtoValidator>();
            services.AddTransient<IValidator<InvoiceDto>, InvoiceDtoValidator>();
            services.AddTransient<IValidator<InvoiceChaseDto>, InvoiceChaseDtoValidator>();
            services.AddTransient<IValidator<InvoiceStatusDto>, InvoiceStatusDtoValidator>();
            services.AddTransient<IValidator<LeadStatusDto>, LeadStatusDtoValidator>();
            services.AddTransient<IValidator<ConclusionDto>, ConclusionDtoValidator>();
            services.AddTransient<IValidator<LeadDto>, LeadDtoValidator>();
            services.AddTransient<IValidator<ExpenseDto>, ExpenseDtoValidator>();
            services.AddTransient<IValidator<DeviceDataDto>, DeviceDataDtoValidator>();
            services.AddTransient<IValidator<AssignDeviceDto>, AssignDeviceDtoValidator>();
            services.AddTransient<IValidator<ReturnDeviceDto>, ReturnDeviceDtoValidator>();
            services.AddTransient<IValidator<AppraiseDto>, AppraiseDtoValidator>();
            services.AddTransient<IValidator<ComplaintDto>, ComplaintDtoValidator>();
            services.AddTransient<IValidator<TechnologyDto>, TechnologyDtoValidator>();
            services.AddTransient<IValidator<BugReportDto>, BugReportDtoValidator>();
            services.AddTransient<IValidator<BucketModelDto>, BucketModelDtoValidator>();
            services.AddTransient<IValidator<LeadStatusModelDto>, LeadStatusModelDtoValidator>();
            services.AddTransient<IValidator<BugStatusDto>, BugStatusDtoValidator>();
            services.AddTransient<IValidator<ProjectClosureReviewDto>, ProjectClosureReviewDtoValidator>();
            services.AddTransient<IValidator<MomMeetingTaskDto>, MomMeetingTaskDtoValidator>();
            services.AddTransient<IValidator<MomMeetingDto>, MomMeetingDtoValidator>();
            services.AddTransient<IValidator<MomMeetingTaskCommentsAddDto>, MomMeetingTaskCommentsAddDtoValidator>();
            services.AddTransient<IValidator<LibraryDto>, LibraryDtoValidator>();
            services.AddTransient<IValidator<LibraryDownloadDto>, LibraryDownloadDtoValidator>();
            services.AddTransient<IValidator<ProjectClientFeedbackDetailDto>, ProjectClientFeedbackDetailDtoValidator>();
            services.AddTransient<IValidator<OrgImprovementDto>, OrgImprovementDtoValidator>();
            services.AddTransient<IValidator<SelfDeclarationDto>, SelfDeclarationDtoValidator>();
            services.AddTransient<IValidator<LessonLearntDto>, LessonLearntDtoValidator>();
            services.AddTransient<IValidator<EstimateHourDto>, EstimateHourDtoValidator>();
            services.AddTransient<IValidator<EstimateCalculationDto>, EstimateCalculationDtoValidator>();
            services.AddTransient<IValidator<EstimateFormDto>, EstimateFormDtoValidator>();
            services.AddTransient<IValidator<ChangeUserPasswordDto>, ChangeUserPasswordDtoValidator>();
            services.AddTransient<IValidator<EstimateHostingPackageDto>, EstimateHostingPackageDtoValidator>();
            services.AddTransient<IValidator<SalesKitTypeDto>, SalesKitDtoValidator>();
            services.AddTransient<IValidator<CvsTypeDto>, CVsDtoValidator>();
            services.AddTransient<IValidator<CVEstimatePriceDto>, CVEstimatePriceDtoValidator>();
            //services.AddTransient<IValidator<EscalationDto>, EscalationDtoValidator>();
            services.AddTransient<IValidator<StudyDocumentsDto>, StudyDocumentsDtoValidator>();
            services.AddTransient<IValidator<StudyDocumentFilesDto>, StudyDocumentFilesDtoValidator>();
            services.AddTransient<IValidator<StudyDocumentsPermissionDto>, StudyDocumentsPermissionsDtoValidator>();
            services.AddTransient<IValidator<UserPermissionsDto>, UserPermissionsDtoValidator>();
            services.AddTransient<IValidator<StudyDocumentAddDelUsersPermission>, StudyDocumentAddDelUsersPermissionValidator>();
            services.AddTransient<IValidator<StudyDocumentsUnapprovedReasonDto>, UnapprovedReasonDtoValidator>();
            services.AddTransient<IValidator<ReminderDto>, ReminderDtoValidator>();
            services.AddTransient<IValidator<SmeDto>, SmeDtoValidator>();
        }
    }
}