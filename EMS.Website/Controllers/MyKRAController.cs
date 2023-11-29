using EMS.Data;
using EMS.Dto;
using EMS.Dto.KRA;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Controllers;
using EMS.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class MyKRAController : BaseController
    {
        #region Reference Variables
        private readonly IRoleService _roleService;
        private readonly IKRAServices _kraServices;
        #endregion
        #region "Constructor"
        public MyKRAController(IRoleService roleService, IKRAServices kraServices)
        {
            _roleService = roleService;
            _kraServices = kraServices;
        }
        #endregion
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Index()
        {
            KraDtoGroup kraDtoGroup= new KraDtoGroup();
            var roleCategoryId = _roleService.GetRoleById(CurrentUser.RoleId).RoleCategoryId;
            if (roleCategoryId.HasValue)
            {
                List<int> roleIds = _roleService.GetRolesByRoleCategoyId(roleCategoryId.Value).Select(x => x.RoleId).ToList();
               
                kraDtoGroup.KRAList = _kraServices.GetKradataByDesgnationId(CurrentUser.DesignationId);
                var _designation = _kraServices.GetDesignationById(CurrentUser.DesignationId);
                if (_designation != null)
                {
                    var designations = _roleService.GetDesignationByGroupId(_designation.Designation.GroupId.Value).OrderBy(a => a.DisplayOrder);

                    var userDesignation = designations.Where(x => x.Id == CurrentUser.DesignationId).FirstOrDefault();
                    kraDtoGroup.Name = userDesignation.Name;
                    kraDtoGroup.DesignationList = designations.ToList();
                    if (userDesignation.ParentGroupId != null)
                    {
                        var parentDesignations = _roleService.GetDesignationByParentGroupIds(userDesignation.ParentGroupId.Value);

                        if (parentDesignations.Count > 0)
                        {
                            foreach (var item in parentDesignations.OrderBy(x => x.DisplayOrder))
                            {
                                Designation des = new Designation();
                                des.Id = item.Id;
                                des.Name = item.Name;
                                des.DisplayOrder = item.DisplayOrder;
                                des.ParentGroupId = item.ParentGroupId;
                                des.GroupId = item.GroupId;
                                kraDtoGroup.DesignationList.Add(des);
                            }

                        }

                    }

                    if (_designation != null)
                    {
                        kraDtoGroup.DesignationName = _designation.Designation.Name;
                    }
                }
            }
            return View(kraDtoGroup);
        }

       
        public IActionResult GetKRA(int Id)
        {
            var KRAs = _kraServices.GetKradataByDesgnationId(Id).ToList();
            DesignationKRAViewModel model = new DesignationKRAViewModel();
            List<DesignationKRA> designationKraLst = new List<DesignationKRA>();
            if (KRAs != null && KRAs.Count() > 0)
            {
                foreach (var item in KRAs)
                {
                    DesignationKRA designationKra = new DesignationKRA();
                    designationKra.Title = item.Title;
                    designationKra.Description = item.Description;
                    designationKra.DisplayOrder = item.DisplayOrder;
                    designationKra.DesignationId = item.DesignationId;
                    designationKraLst.Add(designationKra);
                }
                model.DesignationTitle = KRAs.FirstOrDefault().Designation.Name;
            }
            else
            {
                DesignationKRA designationKra = new DesignationKRA();
                designationKra.Title = "KRA record will be updated very soon for selected designation.";
                designationKra.Description = string.Empty;
                designationKra.DisplayOrder = 1;
                designationKra.DesignationId = Id;
                designationKraLst.Add(designationKra);
            }
            
            
            model.DesignationKRAList = designationKraLst;
            //return View(designationKraLst);
            return PartialView("_KRADetails", model);
        }

    }
}
