using System.Collections.Generic;

namespace EMS.Website.Models
{
    public class DesignationKRA
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int? DesignationId { get; set; }
        public int? DisplayOrder { get; set; }
    }
    public class DesignationKRAViewModel {
        public string DesignationTitle { get; set; }
        public List<DesignationKRA> DesignationKRAList { get; set; }
    }
}
