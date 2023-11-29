using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class LibraryPermissionListDto
    {
        public LibraryPermissionListDto()
        {
            LibraryFileTypeList = new List<LibraryFileTypeIdDto>();
        }
        public int rowIndex { get; set; }
        public int? RoleId { get; set; }
        public int? UserLoginId { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        //public List<LibraryFileType> FileTypeList { get; set; }

        public List<LibraryFileTypeIdDto> LibraryFileTypeList { get; set; }
    }



    public class LibraryFileTypeIdDto
    {
        public LibraryFileTypeIdDto() { }
        public int LibraryFileTypeId { get; set; }
        public string LibraryFileTypeName { get; set; }
        public string MaximumDownloadInDay { get; set; }
        public string MaximumDownloadInMonth { get; set; }
    }


}
