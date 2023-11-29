using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class ProjectClousreDetailDto
    {
        public int Id { get; set; }
        public int ProjectClousreId { get; set; }     
        public string NextStartDate { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AddedByUid { get; set; }
        public int PMUid { get; set; }
        public bool IsConfirmSubmit { get; set; } = false;
        public bool ExpectedNewWork { get; set; } = false;
        public string ConversionDate { get; set; }
    }
}
