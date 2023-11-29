using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class KRAController : BaseController
    {

        #region "Fields"       
        private IKRAServices kraServices;
        private IRoleService roleServices;
        #endregion

        #region "Constructor"
        public KRAController(IKRAServices kraServices, IRoleService roleServices)
        {
            this.kraServices = kraServices;
            this.roleServices = roleServices;

        }
        #endregion

        [HttpGet]
        public IActionResult Index()
        {
            KraDto kraDto = new KraDto();

            List<RoleCategory> roleCategoryList = roleServices.GetActiveRoleCategory();
            kraDto.roleCategoryList = roleServices.GetActiveRoleCategory().Select(x => new DropdownListDto { Text = x.Name, Id = x.Id }).ToList();


            List<Role> roleList = roleServices.GetActiveRoles();
            kraDto.RoleList = roleServices.GetActiveRoles().Select(x => new DropdownListDto { Text = x.RoleName, Id = x.RoleId }).ToList();



            List<Designation> designationList = roleServices.GetDesignationList();
            kraDto.designationList = roleServices.GetDesignationList().Select(x => new DropdownListDto { Text = x.Name, Id = x.Id }).ToList();

            return View(kraDto);
        }


        [HttpGet]
        public ActionResult GetKRAData(int DesignationId)
        {
            KraDto kraDto = new KraDto();

            var data = kraServices.GetDesignationById(DesignationId);
            if (data!=null)
            {
                kraDto.DesignationId = data.DesignationId;
                kraDto.CreatedBy = data.CreatedBy;
                //kraDto.RoleCateGoryId = data.;
                kraDto.Status = data.IsActive.HasValue ? data.IsActive.Value : false;
                kraDto.dataList = kraServices.GetKradataByDesgnationId(DesignationId).Select(x => new KRAData { Title = x.Title, KRAOrderno = x.DisplayOrder }).ToList();
            }
            else
            {
                return Json(null);
            }
            return Json(kraDto);
        }
       



        [HttpPost]
        public ActionResult SaveRecords(string jsondata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    KraDto KraDto = new KraDto();
                    KraDto = JsonConvert.DeserializeObject<KraDto>(jsondata);
                    kraServices.DeleteList(KraDto);
                    if (KraDto != null)
                    {
                        KraDto.CreatedBy = CurrentUser.Uid;
                        kraServices.SaveList(KraDto);
                    }

                    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index", "KRA"), Message = "Record saved successfully.", IsSuccess = true });
                }
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                ModelState.AddModelError("Error!", message);
            }
            return CreateModelStateErrors();
        }

        [HttpGet]
        public ActionResult BindRoleList(int? RoleCateGoryId)
        {
            var ddlRole = roleServices.GetRolesByRoleCategoyId(RoleCateGoryId ?? 0);
            var ddlRoleList = ddlRole.Select(x => new SelectListItem() { Text = x.RoleName, Value = x.RoleId.ToString() }).ToList();
            ddlRoleList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });

            var response = new
            {
                RoleList = ddlRoleList,
            };


            return Json(response);

        }

        [HttpGet]
        public ActionResult BindDesignationList(int? RoleId)
        {
            var ddlDesignation = roleServices.GetDesignationByRoleId(RoleId ?? 0);
            var ddlDesignationList = ddlDesignation.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ddlDesignationList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });

            var response = new
            {
                DesignationList = ddlDesignationList,
            };
            return Json(response);

        }
    }
}
