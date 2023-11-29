using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Component
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte Type { get; set; }
        public int ComponentCategoryId { get; set; }
        public string Tags { get; set; }
        public string ImageName { get; set; }
        public string Description { get; set; }
        public string DataUrl { get; set; }
        public string DesignImages { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByUid { get; set; }
        public string PsdImages { get; set; }

        public virtual ComponentCategory ComponentCategory { get; set; }
        public virtual UserLogin UserLogin { get; set; }
    }
}
