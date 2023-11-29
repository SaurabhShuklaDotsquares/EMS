using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using EMS.Web.Code.LIBS;
using EMS.Service;
using EMS.Data;
using DataTables.AspNet.Core;
using EMS.Dto;
using EMS.Core;
using EMS.Web.Code.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class VirtualDeveloperController : BaseController
    {
        private IVirtualDeveloperService virtualDeveloperServices;
        private IUserLoginService userLoginServices;
        public VirtualDeveloperController(IVirtualDeveloperService virtualDeveloperServices, IUserLoginService userLoginServices)
        {
            this.virtualDeveloperServices = virtualDeveloperServices;
            this.userLoginServices = userLoginServices;
        }

        [CustomActionAuthorization]
        // GET: VirtualDeveloper
        public ActionResult Index()
        {
            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
            {
                var pmList = userLoginServices.GetUserByRole((int)Enums.UserRoles.PM).Select(x => new SelectListItem
                {
                    Text = x.Name.ToString(),
                    Value = x.Uid.ToString()
                }).ToList();
                pmList.InsertRange(pmList.Count, userLoginServices.GetUserByRole((int)Enums.UserRoles.PMO).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList());
                ViewBag.pmList = pmList;
            }

            return View();
        }
        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, string searchName, string ddl_pmUid)
        {
            int pmUid = ddl_pmUid == null ? 0 : Convert.ToInt32(ddl_pmUid);
            var pagingService = new PagingService<VirtualDeveloper>(request.Start, request.Length);
            var expr = PredicateBuilder.True<VirtualDeveloper>();
            if (!String.IsNullOrEmpty(searchName))
            {
                expr = expr.And(e => e.VirtualDeveloper_Name.Contains(searchName));
            }

            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO)
            {
                expr = expr.And(e => e.PMUid == CurrentUser.Uid);
            }
            else if (CurrentUser.RoleId != (int)Enums.UserRoles.HRBP)
            {
                expr = expr.And(e => e.PMUid == CurrentUser.PMUid);
            }
            if (pmUid != 0)
            {
                expr = expr.And(e => e.PMUid == pmUid);
            }

            pagingService.Filter = expr;

            pagingService.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.VirtualDeveloper_Name);
                        default:
                            return o.OrderByColumn(item, c => c.ModifiedDate);
                    }
                }
                return o.OrderByDescending(c => c.ModifiedDate);
            };
            int totalCount = 0;
            var response = virtualDeveloperServices.GetVirtualDeveloperByPaging(out totalCount, pagingService);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                r.VirtualDeveloper_ID,
                rowId = (index + 1) + (request.Start),
                r.VirtualDeveloper_Name,
                r.emailid,
                r.isactive
            }));
        }
        [HttpGet]
        public ActionResult UpdateStatus(int id)
        {
            if (id != 0)
            {
                VirtualDeveloper virtualDeveloper = new VirtualDeveloper();
                virtualDeveloper = virtualDeveloperServices.GetVirtualDeveloperById(id);
                if (virtualDeveloper.isactive == true)
                {
                    virtualDeveloper.isactive = false;
                }
                else
                {
                    virtualDeveloper.isactive = true;
                }
                virtualDeveloperServices.Save(virtualDeveloper);
            }
            return RedirectToAction("Index", "VirtualDeveloper");
        }
        [HttpGet]
        public ActionResult AddEditDeveloper(int? id)
        {
            VirtualDeveloperDto virtualDeveloperViewModel = new VirtualDeveloperDto();
            if (id != null)
            {
                VirtualDeveloper virtualDeveloper = new VirtualDeveloper();
                virtualDeveloper = virtualDeveloperServices.GetVirtualDeveloperById(Convert.ToInt32(id));
                virtualDeveloperViewModel.VirtualDeveloper_Id = virtualDeveloper.VirtualDeveloper_ID;
                virtualDeveloperViewModel.VirtualDeveloper_Name = virtualDeveloper.VirtualDeveloper_Name;
                virtualDeveloperViewModel.Email = virtualDeveloper.emailid;
                virtualDeveloperViewModel.PMUid = Convert.ToInt32(virtualDeveloper.PMUid);
                virtualDeveloperViewModel.IsActive = Convert.ToBoolean(virtualDeveloper.isactive);
            }
            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
            {
                virtualDeveloperViewModel.PMList = userLoginServices.GetPMAndPMOUsers(true).Select(x => new SelectListItem
                {
                    Text = x.Name.ToString(),
                    Value = x.Uid.ToString()
                }).ToList();
            }
            return PartialView("_AddEditVirtualDeveloper", virtualDeveloperViewModel);
        }
        [HttpPost]
        public ActionResult AddEditDeveloper(VirtualDeveloperDto virtualDeveloperViewModel)
        {

            if (CurrentUser != null && CurrentUser.Uid != 0 && ModelState.IsValid)
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO)
                {
                  //  CurrentUser.PMUid = CurrentUser.Uid;
                }
                bool success = false;

                VirtualDeveloper virtualDeveloper = new VirtualDeveloper();

                if (virtualDeveloperViewModel.VirtualDeveloper_Id != 0)
                {
                    virtualDeveloper = virtualDeveloperServices.GetVirtualDeveloperById(virtualDeveloperViewModel.VirtualDeveloper_Id);
                }
                virtualDeveloper.VirtualDeveloper_ID = virtualDeveloperViewModel.VirtualDeveloper_Id;
                virtualDeveloper.VirtualDeveloper_Name = virtualDeveloperViewModel.VirtualDeveloper_Name;
                virtualDeveloper.emailid = virtualDeveloperViewModel.Email.Trim().ToLower();
                virtualDeveloper.PMUid = CurrentUser.RoleId == (int)Enums.UserRoles.HRBP ? virtualDeveloperViewModel.PMUid : CurrentUser.PMUid;
                virtualDeveloper.isactive = virtualDeveloperViewModel.IsActive;
                virtualDeveloper.Ismain = true;
                virtualDeveloper.ModifiedDate = DateTime.Now;


                success = virtualDeveloperServices.Save(virtualDeveloper);
                if (success)
                {
                    if (virtualDeveloperViewModel.VirtualDeveloper_Id == 0)
                    {
                        ShowSuccessMessage("Success", "Virtual developer has been successfully added!", false);
                    }
                    else
                    {
                        ShowSuccessMessage("Success", "Virtual developer has been successfully updated!", false);
                    }

                }
                else
                {
                    ShowErrorMessage("Failed", "Virtual developer email Id already exist", false);
                }
            }
            return Json(new { isSuccess = true, redirectUrl = Url.Action("Index", "VirtualDeveloper") });
        }
    }

}