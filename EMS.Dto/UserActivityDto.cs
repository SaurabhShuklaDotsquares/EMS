using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class UserLiteActivityLogDto
    {
        public Nullable<int> Uid { get; set; }
        public DateTime Date { get; set; }
        public List<UserActivityLogProjectDto> Projects { get; set; }
    }
    public class UserActivityLogProjectDto
    {
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
