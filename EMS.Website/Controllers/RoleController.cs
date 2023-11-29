using System;
using System.Linq;
//using System.Web.Mvc;
using EMS.Data;
using EMS.Service;
using EMS.Web.Code.Attributes;
//using DataTables.AspNet.Core;
using EMS.Core;
//using EMS.Web.Code.LIBS;
using EMS.Dto;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class RoleController : BaseController
    {
        #region "Fields"       
        private IRoleService roleServices;
        #endregion

        #region "Constructor"
        public RoleController(IRoleService roleServices)
        {
            this.roleServices = roleServices;

        }
        #endregion

      [CustomActionAuthorization]
        [HttpGet]
        public ActionResult Index()
        {
            RoleDto roleDto = new RoleDto();
           

            List<RoleCategory> roleCategoryList = roleServices.GetActiveRoleCategory();
            roleDto.roleCategoryList = roleServices.GetActiveRoleCategory().Select(x => new DropdownListDto { Text = x.Name, Id = x.Id }).ToList();

            


            List<Role> roleList = roleServices.GetActiveRoles();
            roleDto.RoleList = roleServices.GetActiveRoles().Select(x => new DropdownListDto { Text = x.RoleName, Id = x.RoleId }).ToList();



            List<Designation> designationList = roleServices.GetDesignationList();
            roleDto.designationList = roleServices.GetDesignationList().Select(x => new DropdownListDto { Text = x.Name, Id = x.Id }).ToList();



            return View(roleDto);
        }

      [CustomActionAuthorization]
        [HttpGet]
        public ActionResult MenuAccess(int roleId, int designationId)
        {
            RoleDto roleDto = new RoleDto();

            var data = roleServices.GetRoleById(roleId);
            roleDto.RoleName = data.RoleName;
            roleDto.RoleId = data.RoleId;
          //  roleDto.DesignationId = data.Designation.I;
            //roleDto.RoleCateGoryId = data.RoleCategory.Id;
            roleDto.Status = data.IsActive.HasValue ? data.IsActive.Value : false;

            roleDto.ParentMenu = roleServices.GetParentMenu().Select(x => new ParentMenuLDto { MenuId = x.MenuId, MenuDisplayName = Regex.Replace(x.MenuDisplayName, @"\s", "") }).ToList();

            var itemList = roleDto.ParentMenu.Select(x => x.MenuId).ToList();

            roleDto.AllMenu = roleServices.GetFrontMenuByIds(itemList).Select(x => new AllMenuDto { MenuDisplayName = x.MenuDisplayName.Trim(), MenuId = x.MenuId, ParentId = x.ParentId.HasValue ? x.ParentId.Value : 0 }).ToList();

            roleDto.ChildMenuList = roleServices.GetChildMenu(roleId, designationId).Select(x => new ChildMenuLDto { MenuDisplayName = x.FrontMenu.MenuDisplayName.Trim(), MenuId = x.FrontMenu.MenuId, ParentId = x.FrontMenu.ParentId.HasValue ? x.FrontMenu.ParentId.Value : 0 }).ToList();


            roleDto.RoleList = roleServices.GetActiveRoles().Select(x => new DropdownListDto { Text = x.RoleName, Id = x.RoleId }).ToList();

            return View("_MenuAccess", roleDto);
        }
        [HttpPost]

        public ActionResult SaveRecords(RoleDto roleData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    roleServices.DeleteList(roleData);
                    if (roleData.childmenu != null)
                    {
                        roleServices.SaveList(roleData);
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index", "role"), Message = "Record saved successfully.", IsSuccess = true });
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