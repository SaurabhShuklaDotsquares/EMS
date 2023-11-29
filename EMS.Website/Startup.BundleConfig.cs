using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EMS.Website
{
    public partial class Startup
    {
        public void AddWebOptimizer(IServiceCollection services)
        {
            services.AddWebOptimizer(bundles =>
            {
                bundles.AddCssBundle("/Content/layoutcss",
                    "/Content/css/bootstrap.min.css",
                    "/Content/css/font-awesome.min.css",
                    "/Content/css/style.css",
                    "/Scripts/datatables/dataTables.bootstrap.css",
                    "/Content/css/responsive.css",
                    "/Styles/jquery-ui/jquery-ui.css",
                    "/Styles/alertmessages.css",
                    "/css/chosen.css",
                    "/css/select2.css",
                    "/Scripts/plugin/jquery-confirm.css"
                   );

                bundles.AddJavaScriptBundle("/bundles/jquery", "/Scripts/jquery-1.10.2.min.js");
                bundles.AddJavaScriptBundle("/bundles/jqueryval",
                    "/lib/jquery-validation/dist/jquery.validate.js",
                    "/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js");
                bundles.AddJavaScriptBundle("/bundles/modernizr", "/Scripts/modernizr-*");
                bundles.AddJavaScriptBundle("/bundles/bootstrap", "/Scripts/bootstrap.min.js", "/Scripts/respond.js");

                bundles.AddJavaScriptBundle("/bundles/otherjs",
                      "/Scripts/jquery.dataTables.js",
                      "/Scripts/datatables/dataTables.bootstrap.js",
                      "/Scripts/chosen.jquery.js",
                      "/Scripts/plugin/jquery-confirm.min.js");

                bundles.AddJavaScriptBundle("/bundles/blockUI",
                      "/Scripts/jquery.blockUI.js",
                      "/Scripts/AjaxLoader.js");

                bundles.AddJavaScriptBundle("/bundles/layoutjs",
                      "/Scripts/additional-methods.js",
                      "/Scripts/jquery-ui.js",
                      "/Scripts/global.js",
                      "/Content/js/iphone-menu.js");

                AddViewPageScriptBundles(bundles);


                //bundles.MinifyCssFiles();
            });
        }

        private static void AddViewPageScriptBundles(WebOptimizer.IAssetPipeline bundles)
        {

            bundles.AddJavaScriptBundle("/bundles/MeetingRoom/index",
                  "/Scripts/plugin/FullCalender/Js/moment.min.js",
                  "/Scripts/plugin/FullCalender/Js/fullcalendar.min.js",
                  "/Scripts/plugin/FullCalender/Js/bootstrap-datetimepicker.min.js",
                  "/Scripts/CustomJS/MeetingRoom/Index.js"
                  );

            bundles.AddJavaScriptBundle("/bundles/Project/index",
                  "/Scripts/datatables/dataTables.bootstrap.js",
                  "/Scripts/knockout-3.3.0.js",
                  "/Scripts/knockout.mapping-latest.js",
                  "/Scripts/CustomJS/Project/Index.js"
                  );
            bundles.AddJavaScriptBundle("/bundles/User/manageUser",
                 //"/Scripts/datatables/dataTables.bootstrap.js",
                  "/Scripts/knockout-3.3.0.js",
                  "/Scripts/knockout.mapping-latest.js",
                "/Scripts/CustomJS/manageUser.js");
            bundles.AddJavaScriptBundle("/bundles/Project/updateAdditionalSupport",
                 "/Scripts/CustomJS/Project/UpdateAdditionalSupport.js"
                  );

            bundles.AddJavaScriptBundle("/bundles/OrgDocument/review",
                 "/Scripts/datatables/dataTables.bootstrap.js",
                 "/Scripts/CustomJS/OrgDocument/review.js"
                  );

            bundles.AddJavaScriptBundle("/bundles/LessonLearned/index",
                "/Scripts/datatables/dataTables.bootstrap.js",
                "/Scripts/CustomJS/LessonLearned/index.js");

            bundles.AddJavaScriptBundle("/bundles/NCLog/index",
                "/Scripts/datatables/dataTables.bootstrap.js",
                "/scripts/select2.min.js",
                "/Scripts/CustomJS/NcLog/index.js");

            bundles.AddJavaScriptBundle("/bundles/PILog/index",
                "/Scripts/datatables/dataTables.bootstrap.js",
                "/Scripts/CustomJS/PILog/index.js");

            bundles.AddJavaScriptBundle("/bundles/Select2", "/scripts/select2.min.js");

            bundles.AddJavaScriptBundle("/bundles/DataTimePicker",
              "/Scripts/datepicker/moment.js",
              "/Scripts/plugin/Datepicker/bootstrap-datetimepicker.js");

            bundles.AddJavaScriptBundle("/bundles/Home/index",
                "/Scripts/knockout-3.3.0.js",
                "/Scripts/knockout.mapping-latest.js");

            bundles.AddJavaScriptBundle("/bundles/Device/deviceMaster", "/Scripts/CustomJS/Device/device-master.js");

            bundles.AddJavaScriptBundle("/bundles/Device/index", "/Scripts/CustomJS/Device/index.js");

            bundles.AddJavaScriptBundle("/bundles/ProductLandingPage/index", "/Scripts/CustomJS/ProductLandingPage/index.js");

            bundles.AddJavaScriptBundle("/bundles/ProductLandingPage/addEdit",
                "/Scripts/plugin/fine-uploader/fine-uploader.min.js",
                "/Scripts/CustomJS/ProductLandingPage/AddEdit.js");

            bundles.AddJavaScriptBundle("/bundles/Activity/activity", "/Scripts/CustomJS/Activity.js");
            bundles.AddJavaScriptBundle("/bundles/Estimate/addEditLead", "/Scripts/CustomJS/addEditLead.js");

            bundles.AddJavaScriptBundle("/bundles/ProjectClosure/index",
                "/Scripts/plugin/FullCalender/Js/moment.min.js",
                "/Scripts/plugin/DateRangePicker/daterangepicker.js",
                "/Scripts/CustomJS/ProjectClosure/projectClosure-index.js");

            bundles.AddJavaScriptBundle("/bundles/ProjectClosure/report",
                "/Scripts/plugin/FullCalender/Js/moment.min.js",
                "/Scripts/plugin/DateRangePicker/daterangepicker.js",
                "/Scripts/CustomJS/ProjectClosure/projectClosure-report.js");

            bundles.AddJavaScriptBundle("/bundles/Home/Moment",
                "/Scripts/plugin/FullCalender/Js/moment.min.js",
                "/Scripts/plugin/DateRangePicker/daterangepicker.js");

            bundles.AddJavaScriptBundle("/bundles/ProjectClosure/review",
                "/Scripts/plugin/FullCalender/Js/moment.min.js",
                "/Scripts/plugin/DateRangePicker/daterangepicker.js",
                "/Scripts/CustomJS/ProjectClosure/projectClosure-review.js");

            bundles.AddJavaScriptBundle("/bundles/ProjectClosure/create",
                "/scripts/select2.min.js",
                "/Scripts/CustomJS/ProjectClosure/projectClosure-create.js");


            bundles.AddJavaScriptBundle("/bundles/MedicalData",
              "/Scripts/plugin/FullCalender/Js/moment.min.js",
              "/Scripts/plugin/DateRangePicker/daterangepicker.js",
              "/Scripts/CustomJS/medicaldata/Index.js");

    
            bundles.AddJavaScriptBundle("/bundles/User/manageProfile", "/Scripts/CustomJS/ManageProfile.js");
            bundles.AddJavaScriptBundle("/bundles/OrgDocument/addEdit", "/Scripts/CustomJS/OrgDocument/AddEdit.js");
            bundles.AddJavaScriptBundle("/bundles/LessonLearned/addEdit", "/Scripts/CustomJS/LessonLearned/AddEdit.js");
            bundles.AddJavaScriptBundle("/bundles/Appraise/index", "/Scripts/CustomJS/Appraise/index.js");
            bundles.AddJavaScriptBundle("/bundles/Complaint/index", "/Scripts/CustomJS/Complaint/index.js");
            bundles.AddJavaScriptBundle("/bundles/BugReport/index", "/Scripts/CustomJS/BugReport/index.js");
            bundles.AddJavaScriptBundle("/bundles/Invoice/index", "/Scripts/CustomJS/Invoice/invoice-index.js");
            bundles.AddJavaScriptBundle("/bundles/Invoice/invoiceList", "/Scripts/CustomJS/Invoice/project-invoicelist.js");
            bundles.AddJavaScriptBundle("/bundles/LeadStatusModel/index", "/Scripts/CustomJS/LeadStatusModel/LeadStatusModel.js");
            bundles.AddJavaScriptBundle("/bundles/TeamStructure/index", "/Scripts/CustomJS/TeamStructure/index.js");
            bundles.AddJavaScriptBundle("/bundles/DailyThought/index", "/Scripts/CustomJS/DailyThought/index.js");
            bundles.AddJavaScriptBundle("/bundles/Complaint/addEdit", "/Scripts/CustomJS/Complaint/AddEdit.js");
            bundles.AddJavaScriptBundle("/bundles/ProjectInfo/index", "/Scripts/CustomJS/ProjectInfo/Index.js");
            bundles.AddJavaScriptBundle("/bundles/Report/ProjectUserReport", "/Scripts/CustomJS/Report/ProjectUserReport.js");
            bundles.AddJavaScriptBundle("/bundles/Report/UserReport", "/Scripts/CustomJS/Report/UserReport.js");
            bundles.AddJavaScriptBundle("/bundles/Report/WorkingHourReport", "/Scripts/CustomJS/Report/WorkingHour.js");
            bundles.AddJavaScriptBundle("/bundles/MOM/viewtask", "/Scripts/CustomJS/MOM/viewtask.js");
            bundles.AddJavaScriptBundle("/bundles/MOM/index", "/Scripts/CustomJS/MOM/index.js");
            bundles.AddJavaScriptBundle("/bundles/Project/additionalSupport", "/Scripts/CustomJS/Project/AdditionalSupport.js");
            bundles.AddJavaScriptBundle("/bundles/User/birthdays", "/Scripts/CustomJS/birthdays.js");
            bundles.AddJavaScriptBundle("/bundles/ProjectClientFeedback/index", "/Scripts/CustomJS/ProjectClientFeedback/index.js");
            bundles.AddJavaScriptBundle("/bundles/EmployeeFeedback",
                "/Scripts/plugin/DateRangePicker/daterangepicker.js",
              "/Scripts/CustomJS/EmployeeFeedback/feedback.js");
            //bundles.AddJavaScriptBundle("/bundles/LibraryManagement/Index",
            //    "/Scripts/CustomJS/LibraryManagement/LibraryManagement-index.js",
            //    "/Scripts/CustomJS/LibraryManagement/LibraryManagement-AdvanceSearch.js");
            bundles.AddJavaScriptBundle("/bundles/OrgImprovement/Index", "/Scripts/CustomJS/OrgImprovement/orgImprovement.js");
            bundles.AddJavaScriptBundle("/bundles/Performance/Index", "/Scripts/CustomJS/Performance/performance.js");
            bundles.AddJavaScriptBundle("/bundles/Report/EmployeeWorkingHour", "/Scripts/CustomJS/Report/EmployeeWorkingHours.js");
            bundles.AddJavaScriptBundle("/bundles/Report/EmployeeWorkingHour2", "/Scripts/CustomJS/Report/EmployeeWorkingHours2.js");
            bundles.AddJavaScriptBundle("/bundles/ProjectClosure/reportCOVID19",
                "/Scripts/plugin/FullCalender/Js/moment.min.js",
                "/Scripts/plugin/DateRangePicker/daterangepicker.js",
                "/Scripts/CustomJS/ProjectClosure/projectClosure-reportCOVID19.js");
            bundles.AddJavaScriptBundle("/bundles/lessonlearnt/index", "/Scripts/CustomJS/lessonlearnt/index.js");
            //bundles.AddJavaScriptBundle("/bundles/EmployeeFeedback/add", "/Scripts/CustomJS/EmployeeFeedback/feedback-addEdit.js",
            //    "/scripts/jquery-ui-timepicker.js", "~/scripts/select2.min.js");
            bundles.AddJavaScriptBundle("/bundles/User/SetEncryptPass", "/Scripts/CustomJS/User/SetEncryptPass.js");
            //bundles.MinifyJsFiles();
        }


    }
}