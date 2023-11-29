using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EMS.Web.Code.Attributes;
using System.Web.Mvc;
using System.Globalization;
using EMS.Web.LIBS;
using System.Transactions;
using EMS.Data;
using EMS.Web.Code.LIBS;
using EMS.Service;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class DepartmentController : BaseController
    {
        // GET: Department
        private IDepartmentService departmentServices;

        public DepartmentController(IDepartmentService departmentServices)
        {
            this.departmentServices = departmentServices;
        }

        [CustomActionAuthorization()]
        [HttpGet]
        public ActionResult index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult index(IDataTablesRequest request, string deptName)
        {
            var pagingService = new PagingService<Department>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Department>();
            if (!String.IsNullOrEmpty(deptName))
            {
                expr = expr.And(e => e.Name.ToLower().Contains(deptName));
            }
            pagingService.Filter = expr;
            pagingService.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.Name);
                        default:
                            return o.OrderByColumn(item, c => c.Name);
                    }
                }
                return o.OrderByDescending(c => c.ModifyDate);
            };
            int totalCount = 0;
            var response = departmentServices.GetDepartments(pagingService);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                r.DeptId,
                rowId = (index + 1) + (request.Start),
                r.Name,
                r.Deptcode,
                r.IsActive
            }));
        }
        [HttpGet]
        public ActionResult AddEditDepartment(int? deptId)
        {
            DepartmentDto departmentViewModel = new DepartmentDto();

            if (deptId != null)
            {
                Department department = departmentServices.GetDepartmentById(Convert.ToInt32(deptId));
                departmentViewModel.DeptId = department.DeptId;
                departmentViewModel.Name = department.Name;
                departmentViewModel.Deptcode = department.Deptcode.ToUpper();
            }


            return PartialView("_AddEditDepartment", departmentViewModel);
        }

        [HttpPost]
        public ActionResult AddEditDepartment(DepartmentDto departmentViewModel)
        {
            if (CurrentUser != null && CurrentUser.Uid != 0)
            {
                if (ModelState.IsValid)
                {
                    //bool IsAlreadyExist = false;
                    bool success = false;
                    if (departmentViewModel.DeptId != null)
                    {

                        Department department = new Department();
                        department = departmentServices.GetDepartmentById(Convert.ToInt32(departmentViewModel.DeptId));
                        department.Name = departmentViewModel.Name.Trim();
                        department.Deptcode = departmentViewModel.Deptcode.ToUpper().Trim();
                        department.ModifyDate = DateTime.Now;
                        success = departmentServices.Save(department);
                        if (success)
                        {
                            ShowSuccessMessage("Success", "Department has been successfully updated.", false);
                        }
                        else
                        {
                            ShowErrorMessage("Error", "Failed..!! Department Name and Code must be unique. Please try another one.", false);

                        }
                    }
                    else
                    {
                        Department department = new Department()
                        {
                            Name = departmentViewModel.Name.Trim(),
                            Deptcode = departmentViewModel.Deptcode.ToUpper().Trim(),
                            AddDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            IsActive = departmentViewModel.IsActive,
                             IP = GeneralMethods.Getip()
                        };
                        success = departmentServices.Save(department);
                        if (success)
                        {
                            ShowSuccessMessage("Success", "Department has been successfully added.", false);
                        }
                        else
                        {
                            ShowErrorMessage("Error", "Failed..!! Department Name and Code must be unique. Please try another one", false);
                        }
                    }
                }
            }
            return Json(new { isSuccess = true, redirectUrl = Url.Action("index", "department") });
        }
        [HttpGet]
        public ActionResult UpdateStatus(int id)
        {
            if (id != 0)
            {
                Department department = new Department();
                department = departmentServices.GetDepartmentById(id);
                if (Convert.ToBoolean(department.IsActive))
                {
                    department.IsActive = false;
                }
                else
                {
                    department.IsActive = true;
                }
                departmentServices.Save(department);
            }
            return RedirectToAction("index", "department");
        }
    }
}