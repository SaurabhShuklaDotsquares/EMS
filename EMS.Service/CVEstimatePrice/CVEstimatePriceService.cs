using Castle.Core.Internal;
using EMS.Data;
using EMS.Dto;
using EMS.Dto.CVEstimatePrice;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service.CVEstimatePrice
{
    public class CVEstimatePriceService : ICVEstimatePriceService
    {
        private readonly IRepository<CvBuilderEstimatePrice> repo;
        private readonly IRepository<ExperienceType> repoExperience;
        private Microsoft.Extensions.Configuration.IConfiguration _configuration;
        public CVEstimatePriceService(IRepository<CvBuilderEstimatePrice> repo, IRepository<ExperienceType> repoExperience, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this.repo = repo;
            this.repoExperience = repoExperience;
            _configuration = configuration;
        }
        public CvBuilderEstimatePrice GetPricebyId(int id)
        {
            return repo.FindById(id);
        }

        public List<CvBuilderEstimatePrice> GetExperincePricesbyTechnology(int RoleId, int TechId)
        {
            return repo.Query().Filter(x => x.IsActive == true && x.RoleId == RoleId && x.TechId == TechId).Get().ToList();
        }
        public List<CvBuilderEstimatePrice> GetExperincePricesbyTechnologyId(int RoleId, int TechnologyId)
        {
            return repo.Query().Filter(x => x.IsActive == true && x.RoleId == RoleId && x.TechId == TechnologyId).Get().ToList();
        }
        public List<CvBuilderEstimatePrice> GetExpericnePricesbyRole(int RoleId)
        {
            return repo.Query().Filter(x => x.IsActive == true && x.RoleId == RoleId).Get().ToList();
        }

        public List<CvBuilderEstimatePrice> GetCvBuilderEstimatePrice(out int total,PagingService<CvBuilderEstimatePrice> pagingService)
        {
            return repo.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
            

        }
        public List<CVEstimatePriceResponseDto> GetCvBuilderEstimatePriceSP(int RowStart, int RowEnd, string SearchResult, int ColumnOrder,string OrderDirection)
        {
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_configuration["ConnectionStrings:dsmanagementConnection"]);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
            System.Data.DataSet ds = new System.Data.DataSet();
            cmd = new System.Data.SqlClient.SqlCommand("SpCvEstimatePrice", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RowStart", RowStart);
            cmd.Parameters.AddWithValue("@RowEnd", RowEnd);
            cmd.Parameters.AddWithValue("@SearchResult", SearchResult);
            cmd.Parameters.AddWithValue("@ColumnOrder", ColumnOrder);
            cmd.Parameters.AddWithValue("@OrderDirection", OrderDirection);
            
            da = new System.Data.SqlClient.SqlDataAdapter(cmd);
            da.Fill(ds);
            con.Close();
            List<CVEstimatePriceResponseDto> cvResponse = new List<CVEstimatePriceResponseDto>();
            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
            {
                cvResponse.Add(new CVEstimatePriceResponseDto
                {
                    RoleId = Convert.ToInt32(dr["RoleId"]),
                    TechName = dr["TechName"].ToString(),
                    TechnologyId = Convert.ToInt32(dr["TechId"]),
                    EntryLevel = string.IsNullOrEmpty(dr["1"].ToString())?0: (decimal)dr["1"],
                    onetoTwo = string.IsNullOrEmpty(dr["2"].ToString()) ? 0 : (decimal)dr["2"],
                    threetoSix = string.IsNullOrEmpty(dr["3"].ToString()) ? 0 : (decimal)dr["3"],
                    sixtoTen = string.IsNullOrEmpty(dr["4"].ToString()) ? 0 : (decimal)dr["4"],
                    TenPlus = string.IsNullOrEmpty(dr["5"].ToString()) ? 0 : (decimal)dr["5"],
                });
            }
            
            return cvResponse;
        }
        public CvBuilderEstimatePrice GetPriceListById(int Id)
        {
            return repo.Query().Get().FirstOrDefault(x => x.Id == Id);
        }

        public List<CvBuilderEstimatePrice> GetRecordsById(int RoleId, int TechnologyId)
        {
            return repo.Query().Filter(x => x.RoleId == RoleId && x.TechId == TechnologyId).Get().ToList();
        }

        public List<CvBuilderEstimatePrice> GetRecordsByRoleId(int RoleId)
        {
            return repo.Query().Filter(x => x.RoleId == RoleId).Get().ToList();
        }
        public void Delete(CvBuilderEstimatePrice entity)
        {
            if (entity.RoleId > 0 && entity.TechId > 0)
            {
                repo.ChangeEntityState<CvBuilderEstimatePrice>(entity, ObjectState.Deleted);
                repo.Delete(entity.Id);
            }
        }
        public void DeleteCollection(List<CvBuilderEstimatePrice> entityCollection)
        {
            repo.DeleteCollection(entityCollection);
        }

        public bool Save(CvBuilderEstimatePrice cvBuilderEstimatePrice)
        {
            if (cvBuilderEstimatePrice.Id != 0)
            {
                repo.ChangeEntityState(cvBuilderEstimatePrice, ObjectState.Modified);
                repo.SaveChanges();
            }
            else
            {
                repo.ChangeEntityState(cvBuilderEstimatePrice, ObjectState.Added);
                repo.SaveChanges();
            }
            return true;
        }

        public bool SaveCollection(List<CvBuilderEstimatePrice> entityCollection)
        {
            repo.InsertCollection(entityCollection);
            return true;
        }


    }
}
