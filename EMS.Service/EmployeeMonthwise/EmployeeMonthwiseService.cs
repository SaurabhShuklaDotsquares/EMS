using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Xml.Linq;
using EMS.Dto;
using EMS.Core;
using System.Collections;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;

namespace EMS.Service
{
    public class EmployeeMonthwiseService : IEmployeeMonthwiseService
    {

        private readonly IConfiguration _configuration;

        public EmployeeMonthwiseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<EmployeeMonthwiseModel> GetEmployeeListMonthWise(string JDate, string RDate, string NJDate)
        {
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            List<EmployeeMonthwiseModel> employeeMonthwiseModels = new List<EmployeeMonthwiseModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("dsmanagementConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("GetEmployeeListMonthWise", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.Add("@JDate", SqlDbType.Date).Value = JDate;
                    cmd.Parameters.Add("@RDate", SqlDbType.Date).Value = RDate;
                    cmd.Parameters.Add("@NJDate", SqlDbType.Date).Value = NJDate;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    con.Close();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        employeeMonthwiseModels.Add(new EmployeeMonthwiseModel
                        {
                            IsActive = !string.IsNullOrEmpty(dr["IsActive"].ToString()) ? dr["IsActive"].ToString() : string.Empty,
                            JoinedDate = !string.IsNullOrEmpty(dr["JoinedDate"].ToString()) ? Convert.ToDateTime(dr["JoinedDate"]).ToString("dd-MM-yyyy") : string.Empty,
                            RelievingDate = !string.IsNullOrEmpty(dr["RelievingDate"].ToString()) ? Convert.ToDateTime(dr["RelievingDate"]).ToString("dd-MM-yyyy") : string.Empty,
                            AttendenceId = !string.IsNullOrEmpty(dr["AttendenceId"].ToString()) ? dr["AttendenceId"].ToString() : string.Empty,
                            uid = !string.IsNullOrEmpty(dr["uid"].ToString()) ? dr["uid"].ToString() : string.Empty,
                            Name = !string.IsNullOrEmpty(dr["Name"].ToString()) ? dr["Name"].ToString() : string.Empty,
                            JobTitle = !string.IsNullOrEmpty(dr["JobTitle"].ToString()) ? dr["JobTitle"].ToString() : string.Empty,
                            DepartmentName = !string.IsNullOrEmpty(dr["DepartmentName"].ToString()) ? dr["DepartmentName"].ToString() : string.Empty,
                            PMName = !string.IsNullOrEmpty(dr["PMName"].ToString()) ? dr["PMName"].ToString() : string.Empty,

                        });
                    }
                }
            }
            return employeeMonthwiseModels;
        }

    }
}
