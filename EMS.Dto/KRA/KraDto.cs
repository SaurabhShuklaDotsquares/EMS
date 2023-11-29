using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
   public class KraDto
    {

        public int RoleCateGoryId { get; set; }

        public int RoleId { get; set; }
        public int? DesignationId { get; set; }


        public string Title { get; set; }
        public int? KRAOrderno { get; set; }

        public int? CreatedBy { get; set; }


        public List<DropdownListDto> RoleList { get; set; }
        public List<DropdownListDto> designationList { get; set; }
        public List<DropdownListDto> roleCategoryList { get; set; }

        public List<KRAData> dataList { get; set; }

        public string Value { get; set; }
        public bool Status { get; set; }
    }

    public class KRAData
    {
        public string Title { get; set; }

        public int? KRAOrderno { get; set; }
    }


}
