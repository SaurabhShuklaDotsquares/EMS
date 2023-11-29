using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface IEmployeeMonthwiseService
    {

        List<EmployeeMonthwiseModel> GetEmployeeListMonthWise(string JDate, string RDate, string NJDate);       
    }
}
