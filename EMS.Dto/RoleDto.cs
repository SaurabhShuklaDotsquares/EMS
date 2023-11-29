using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{

    public class RoleDto
    {
       
        public int RoleCateGoryId { get; set; }

        public int RoleId { get; set; }
        public int DesignationId { get; set; }

        

        public string RoleName { get; set; }
        public bool Status { get; set; }
        public int[] childmenu { get; set; }
        public List<ParentMenuLDto> ParentMenu { get; set; }
        public List<ChildMenuLDto> ChildMenuList { get; set; }
        public List<AllMenuDto> AllMenu { get; set; }
        public List<DropdownListDto> RoleList { get; set; }
        public List<DropdownListDto> designationList { get; set; }
        public List<DropdownListDto> roleCategoryList { get; set; }
    }
    public partial class ParentMenuLDto
    {
        public int MenuId { get; set; }
        public int ParentId { get; set; }
        public string MenuName { get; set; }
        public string MenuDisplayName { get; set; }
    }

    public partial class ChildMenuLDto
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int ParentId { get; set; }
        public string MenuName { get; set; }
        public string MenuDisplayName { get; set; }

    }
    public partial class AllMenuDto
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int ParentId { get; set; }
        public string MenuName { get; set; }
        public string MenuDisplayName { get; set; }
    }

}
