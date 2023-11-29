using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.Model
{
    public class RoleCategoryResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class RoleResponseModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int? RoleCategoryId { get; set; }
    }
    public class DesignationResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public string Experience { get; set; }
    }
    public class KRAResponseModel
    {
        public string Title { get; set; }
        public string Designation { get; set; }
    }
}
