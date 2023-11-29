using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using EMS.Web.Code.Attributes;
using static EMS.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class TeamStructureController : BaseController
    {
        private IUserLoginService userLoginService;
        public TeamStructureController(IUserLoginService userLoginService)
        {
            this.userLoginService = userLoginService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            TeamStructureDto model = new TeamStructureDto();
            var users = userLoginService.GetUsersByRole(CurrentUser.Uid, CurrentUser.RoleId, CurrentUser.DesignationId);
            model.Name = CurrentUser.Name;
            model.UserId = CurrentUser.Uid;
            if (CurrentUser.RoleId == (int)UserRoles.PM || CurrentUser.RoleId == (int)UserRoles.HRBP)
            {
                model.IsDropDownVisible = true;
            }
            if (users != null && users.Any())
            {
                model.UserslList = users.Select(x => new SelectListItem()
                {
                    Text = $"{x.Title} {x.Name}",
                    Value = x.Uid.ToString()
                }).ToList();
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult TeamStructure(int? userId)
        {
            UserJsonViewModel listUser = new UserJsonViewModel();
            int maxlimit = 1;
            List<UserLogin> users = new List<UserLogin>();
            int id = 0;

            if (userId.HasValue && userId > 0)
            {
                UserLogin selectedUser = userLoginService.GetUserInfoByID(userId.Value);
                users = userLoginService.GetUsersListForHierarchyByAllRole(selectedUser.Uid, selectedUser.RoleId.Value,selectedUser.DesignationId.Value);
                id = userId.Value;
            }
            else
            {
                users = userLoginService.GetUsersListForHierarchyByAllRole(CurrentUser.Uid, CurrentUser.RoleId,CurrentUser.DesignationId);
                id = CurrentUser.Uid;
            }
            if (users != null)
            {
                var userList = users.Where(x => x.TLId.Equals(id)).ToList();
                var userDetail = userLoginService.GetUsersById(id);
                if (userDetail != null)
                {
                    var list = GenerateTree(userList, users, ref maxlimit);
                    if (list != null)
                    {
                        listUser.children = list;
                        int total = GetNumberofChldren(list);
                        listUser.name = total == 0 ? $"{userDetail.Title} {userDetail.Name}" : $"{userDetail.Title} {userDetail.Name}({total})";
                        maxlimit = maxlimit < listUser.children.Count() ? listUser.children.Count() : maxlimit < 5 ? 5 : maxlimit;
                        listUser.maxLimit = maxlimit;
                    }
                }
            }

            return Json(listUser);
        }

        private List<UserJsonViewModel> GenerateTree(List<UserLogin> tree, List<UserLogin> dt, ref int maxlimit)
        {
            List<UserJsonViewModel> lst = new List<UserJsonViewModel>();
            if (tree.Count > 0)
            {
                var limit = 0;
                foreach (var dr in tree)
                {
                    string treeText = dr.Title + " " + dr.Name;
                    string line = String.Format("<li>{0}", treeText);
                    var subTree = dt.Where(x => x.TLId.Equals(dr.Uid)).ToList();
                    if (subTree.Count > 0)
                    {
                        var children = GenerateTree(subTree, dt, ref maxlimit);
                        limit = limit + children.Count();
                        int total = 0;
                        if (children != null)
                            total = GetNumberofChldren(children);
                        UserJsonViewModel usrjson = new UserJsonViewModel { name = treeText + " (" + total + ")", children = children };
                        lst.Add(usrjson);
                    }
                    else
                    {
                        lst.Add(new UserJsonViewModel { name = treeText });
                    }
                }
                if (limit > maxlimit)
                    maxlimit = limit;
            }
            return lst;
        }

        public int GetNumberofChldren(List<UserJsonViewModel> user)
        {
            int count = 0;
            if (user.Count() > 0)
            {
                user.ForEach(a =>
                {
                    count++;
                    if (a.children != null && a.children.Count() > 0)
                        count = count + GetNumberofChldren(a.children);
                });
            }
            return count;
        }
    }
}