using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ForecastingDepartment
    {
        public int ForecastingId { get; set; }
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual Forecasting Forecasting { get; set; }
    }
}
