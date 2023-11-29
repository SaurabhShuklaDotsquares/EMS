using EMS.Core;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers
{
    [CustomAuthorization]
    public class DailyThoughtController : BaseController
    {
        private readonly IUserLoginService userLoginService;
        public DailyThoughtController(IUserLoginService userLoginService)
        {
            this.userLoginService = userLoginService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            DailyThoughtDto model = new DailyThoughtDto();
            var thought = userLoginService.GetDailyThought();
            if(thought!=null && thought.Id>0)
            {
                model.Id = thought.Id;
                model.Thought1 = thought.Thought1;
                model.Thought2 = thought.Thought2;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DailyThoughtDto model)
        {
            var result = userLoginService.SaveDailyThought(model);
            if(result!= null)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string>
                {
                    IsSuccess = true,
                    Message = "Record saved successfully"
                });
            }
            return NewtonSoftJsonResult(new RequestOutcome<string>
            {
                IsSuccess = false,
                Message = "Record not saved. Please Try again."
            });
        }
    }
}