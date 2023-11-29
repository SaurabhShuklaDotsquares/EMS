using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace EMS.Service
{   
    public class CVBuilderService : ICVBuilderService
{
        private IRepository<Cvbuilder> _repoCvbuilder;
        private IRepository<CvbuilderCoreCompetencies> _repoCvbuilderCoreCompetencies;
        private IRepository<CvbuilderCertifications> _repoCertifications;
        private IRepository<CvBuilderEducation> _repoEducation;
        private IRepository<CvbuilderPreviousExperience> _repoPreviousExperience;
        private IRepository<CvbuilderIndustry> _repoCvbuilderIndustry;
        private IRepository<CvbuilderTechnology> _repoCvbuilderTechnology;
        private IRepository<CvBuilderEstimatePrice> _repoCvBuilderEstimatePrice;
        private Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public CVBuilderService(IRepository<Cvbuilder> repoCvbuilder, IRepository<CvbuilderCoreCompetencies> repoCvbuilderCoreCompetencies,IRepository<CvbuilderCertifications> repoCertifications, IRepository<CvBuilderEducation> repoEducation, IRepository<CvbuilderPreviousExperience> repoPreviousExperience, IRepository<CvbuilderIndustry> repoCvbuilderIndustry, IRepository<CvbuilderTechnology> repoCvbuilderTechnology, IRepository<CvBuilderEstimatePrice> repoCvBuilderEstimatePrice, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _repoCvbuilder = repoCvbuilder; ;
            _repoCvbuilderCoreCompetencies = repoCvbuilderCoreCompetencies;
            _repoCertifications = repoCertifications;
            _repoEducation = repoEducation;
            _repoPreviousExperience = repoPreviousExperience;
            _repoCvbuilderIndustry = repoCvbuilderIndustry;
            _repoCvbuilderTechnology = repoCvbuilderTechnology;
            _repoCvBuilderEstimatePrice = repoCvBuilderEstimatePrice;
            _configuration = configuration;
        }

        public void Save(CVBuilderDto model,string filepath)
        {
            Cvbuilder entity = model.Id > 0 ? _repoCvbuilder.FindById(model.Id) : new Cvbuilder();

            if (entity == null)
            {
                //return null;
            }

            entity.Id = model.Id;
            //entity.Date = model.NextStartDate.ToDateTime("dd/MM/yyyy");
            entity.UserId = model.Uid_User;
            entity.Title = model.Title;
            entity.ProfileSummary = model.ProfileSummary;
            entity.TechnicalSkills = model.TechnicalSkills;
            entity.WorkExperience = model.WorkExperience;
            entity.RolesAcross = model.RolesAcross;
            entity.LinkedinId = model.Linkedin;
            entity.Languages = model.Languages;
            entity.ExperienceId = model.ExperienceType;
            entity.OtherIndustry = model.OtherIndustry;
            entity.OtherTechnology = model.OtherTechnology;
            entity.OtherTechnologyParent = model.OtherTechnologyParent;
            entity.IsActive = true;            
            entity.IsAgree = model.IsAgree;
            if (model.Id > 0)
            {
                _repoCvbuilder.SaveChanges();
            }
            else
            {
                entity.IsApproved = false;
                entity.IsTraining = false;
                entity.CreatedDate = DateTime.Now;
                _repoCvbuilder.Insert(entity);
            }
            var data= cvBuilderFindById(entity.Id);

            if (data != null)
            {
                if (model.dataList != null && model.dataList.Count() > 0)
                {
                    foreach (var item in model.dataList)
                    {
                        CvbuilderCoreCompetencies obj = new CvbuilderCoreCompetencies();
                        obj = _repoCvbuilderCoreCompetencies.Query().Get().Where(x => x.CvbuilderId == data.Id && x.Id==item.Id).FirstOrDefault();                        
                        if (obj != null)
                        {
                            obj.Title = item.Title;
                            obj.LevelId = Convert.ToInt32(item.KRAOrderno);
                        }
                        else
                        {
                            obj = new CvbuilderCoreCompetencies();
                            obj.CvbuilderId = data.Id;
                            obj.Title = item.Title;
                            obj.LevelId = Convert.ToInt32(item.KRAOrderno);
                        }
                        if (obj.Id > 0)
                        {
                            _repoCvbuilderCoreCompetencies.Update(obj);
                        }
                        else
                        {
                            _repoCvbuilderCoreCompetencies.Insert(obj);
                        }
                    }
                    var coreCompetenciesData = _repoCvbuilderCoreCompetencies.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (coreCompetenciesData != null)
                    {
                        List<CvbuilderCoreCompetencies> lstCoreCompetencies = new List<CvbuilderCoreCompetencies>();
                        foreach (var item in coreCompetenciesData)
                        {
                            var isExists = model.dataList.Where(x => x.Id == item.Id || x.Title == item.Title).FirstOrDefault();
                            if (isExists == null)
                            {
                                lstCoreCompetencies.Add(new CvbuilderCoreCompetencies { Id = item.Id, Title = item.Title, CvbuilderId = item.CvbuilderId });
                            }
                        }
                        if (lstCoreCompetencies != null && lstCoreCompetencies.Count > 0)
                        {
                            if (lstCoreCompetencies.Count > 0)
                            {
                                foreach (var entity1 in lstCoreCompetencies)
                                {
                                    _repoCvbuilderCoreCompetencies.Delete(entity1.Id);
                                }
                            }
                        }
                    }
                }
                //if (model.dataList != null && model.dataList.Count > 0)
                //{
                //    var findData = _repoCvbuilderCoreCompetencies.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                //    if (findData != null && findData.Count > 0)
                //    {
                //        _repoCvbuilderCoreCompetencies.DeleteBulk(findData);
                //    }
                //    foreach (var item in model.dataList)
                //    {
                //        if (!string.IsNullOrEmpty(item.Title))
                //        {                            
                //            CvbuilderCoreCompetencies obj = new CvbuilderCoreCompetencies();
                //            obj.CvbuilderId = data.Id;
                //            obj.Title = item.Title;   
                //            _repoCvbuilderCoreCompetencies.Insert(obj);
                //        }
                //    }
                //}
                if (model.Certifications != null && model.Certifications.Count > 0)
                {
                    var findData = _repoCertifications.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (findData != null && findData.Count > 0)
                    {
                        _repoCertifications.DeleteBulk(findData);
                    }
                    int i = 0;
                    foreach (var item in model.Certifications)
                    {
                        if (!string.IsNullOrEmpty(item.Title))
                        {
                            string CertificationImgUrl = string.Empty;
                            if (model.CertificationIMG == null)
                            {
                                CertificationImgUrl = item.CertificationsURL;                                
                            }
                            else
                            {
                                if (item.ImageIndex != "")
                                {
                                    int index = Convert.ToInt32(item.ImageIndex);
                                    IFormFile formFile = model.CertificationIMG[i];
                                    if (formFile != null)
                                    {
                                        string selectedProfileName;
                                        string uniqueFileName = UploadedCertificationFile(formFile, CertificationImgUrl, out selectedProfileName, filepath);
                                        CertificationImgUrl = uniqueFileName;
                                        i++;
                                    }
                                    else
                                    {
                                        CertificationImgUrl = item.CertificationsURL;
                                    }
                                }
                                else
                                {
                                    CertificationImgUrl = item.CertificationsURL;
                                }
                            }
                            CvbuilderCertifications obj = new CvbuilderCertifications();
                            obj.CvbuilderId = data.Id;
                            obj.CertificationName = item.Title;
                            obj.CertificationNumber = item.CertificationsNumber;
                            obj.CertificationsImage = CertificationImgUrl;
                            _repoCertifications.Insert(obj);
                            
                        }
                    }
                }
                
                if (model.Education != null && model.Education.Count > 0)
                {
                    var findData = _repoEducation.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (findData != null && findData.Count > 0)
                    {
                        _repoEducation.DeleteBulk(findData);
                    }
                    foreach (var item in model.Education)
                    {
                        if (!string.IsNullOrEmpty(item.Title))
                        {
                            CvBuilderEducation obj = new CvBuilderEducation();
                            obj.CvbuilderId = data.Id;
                            obj.Title = item.Title;
                            obj.University = item.University;
                            _repoEducation.Insert(obj);
                        }
                    }
                }
                if (model.PreviousExperience != null && model.PreviousExperience.Count > 0)
                {
                    var findData = _repoPreviousExperience.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (findData != null && findData.Count > 0)
                    {
                        _repoPreviousExperience.DeleteBulk(findData);
                    }
                    foreach (var item in model.PreviousExperience)
                    {
                        if (!string.IsNullOrEmpty(item.OrganizationName))
                        {
                            CvbuilderPreviousExperience obj = new CvbuilderPreviousExperience();
                            obj.CvbuilderId = data.Id;
                            obj.OrganizationName = item.OrganizationName;
                            obj.Designation = item.Designation;
                            obj.FromDate = item.FromMonth + " " + item.FromDate;
                            obj.ToDate = item.ToDate != "Present" ? item.ToMonth + " " + item.ToDate : item.ToDate;
                            _repoPreviousExperience.Insert(obj);
                        }
                    }
                }
                
               
            }
            
        }
        private string UploadedCertificationFile(IFormFile ProfileImage, string ProfilePicture, out string selectedFileName,string filepath)
        {
            string uniqueFileName = selectedFileName = null;
            if (ProfileImage != null)
            {
                var extension = System.IO.Path.GetExtension(ProfileImage.FileName);
                if (extension.ToLower() == ".jpeg" || extension.ToLower() == ".jpg" || extension.ToLower() == ".png")
                {
                    string uploadsFolder = Path.Combine(filepath, $"Images//CertificationImage");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    //prev. file
                    string prevpath = Path.Combine(uploadsFolder, ProfilePicture ?? "");
                    if (System.IO.File.Exists(prevpath))
                    {
                        System.IO.File.Delete(prevpath);
                    }
                    //new file
                    selectedFileName = ProfileImage.FileName;
                    uniqueFileName = ProfileImage.FileName.ToUnique();
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfileImage.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }
        public void SaveOld(CVBuilder_Dto model)
        {
            Cvbuilder entity = model.Id > 0 ? _repoCvbuilder.FindById(model.Id) : new Cvbuilder();

            if (entity == null)
            {
                //return null;
            }

            entity.Id = model.Id;
            //entity.Date = model.NextStartDate.ToDateTime("dd/MM/yyyy");
            entity.UserId = model.Uid_User;
            entity.Title = model.Title;
            entity.ProfileSummary = model.ProfileSummary;
            entity.TechnicalSkills = model.TechnicalSkills;
            entity.WorkExperience = model.WorkExperience;
            entity.RolesAcross = model.RolesAcross;
            entity.LinkedinId = model.Linkedin;
            entity.Languages = model.Languages;
            entity.ExperienceId = model.ExperienceType;
            entity.OtherIndustry = model.OtherIndustry;
            entity.OtherTechnology = model.OtherTechnology;
            entity.OtherTechnologyParent = model.OtherTechnologyParent;
            entity.IsActive = true;
            entity.IsAgree = false;
            if (model.Id > 0)
            {
                _repoCvbuilder.SaveChanges();
            }
            else
            {
                entity.IsApproved = false;
                entity.IsTraining = false;
                entity.CreatedDate = DateTime.Now;
                _repoCvbuilder.Insert(entity);
            }
            var data = cvBuilderFindById(entity.Id);

            if (data != null)
            {
                if (model.dataList != null && model.dataList.Count > 0)
                {
                    var findData = _repoCvbuilderCoreCompetencies.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (findData != null && findData.Count > 0)
                    {
                        _repoCvbuilderCoreCompetencies.DeleteBulk(findData);
                    }
                    foreach (var item in model.dataList)
                    {
                        if (!string.IsNullOrEmpty(item.Title))
                        {
                            CvbuilderCoreCompetencies obj = new CvbuilderCoreCompetencies();
                            obj.CvbuilderId = data.Id;
                            obj.Title = item.Title;
                            _repoCvbuilderCoreCompetencies.Insert(obj);
                        }
                    }
                }
                if (model.Certifications != null && model.Certifications.Count > 0)
                {
                    var findData = _repoCertifications.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (findData != null && findData.Count > 0)
                    {
                        _repoCertifications.DeleteBulk(findData);
                    }
                    foreach (var item in model.Certifications)
                    {
                        if (!string.IsNullOrEmpty(item.Title))
                        {
                            CvbuilderCertifications obj = new CvbuilderCertifications();
                            obj.CvbuilderId = data.Id;
                            obj.CertificationName = item.Title;
                            obj.CertificationNumber = item.KRAOrderno;
                            _repoCertifications.Insert(obj);
                        }
                    }
                }
                if (model.Education != null && model.Education.Count > 0)
                {
                    var findData = _repoEducation.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (findData != null && findData.Count > 0)
                    {
                        _repoEducation.DeleteBulk(findData);
                    }
                    foreach (var item in model.Education)
                    {
                        if (!string.IsNullOrEmpty(item.Title))
                        {
                            CvBuilderEducation obj = new CvBuilderEducation();
                            obj.CvbuilderId = data.Id;
                            obj.Title = item.Title;
                            obj.University = item.University;
                            _repoEducation.Insert(obj);
                        }
                    }
                }
                if (model.PreviousExperience != null && model.PreviousExperience.Count > 0)
                {
                    var findData = _repoPreviousExperience.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (findData != null && findData.Count > 0)
                    {
                        _repoPreviousExperience.DeleteBulk(findData);
                    }
                    foreach (var item in model.PreviousExperience)
                    {
                        if (!string.IsNullOrEmpty(item.OrganizationName))
                        {
                            CvbuilderPreviousExperience obj = new CvbuilderPreviousExperience();
                            obj.CvbuilderId = data.Id;
                            obj.OrganizationName = item.OrganizationName;
                            obj.Designation = item.Designation;
                            obj.FromDate = item.FromMonth + " " + item.FromDate;
                            obj.ToDate = item.ToDate != "Present" ? item.ToMonth + " " + item.ToDate : item.ToDate;
                            _repoPreviousExperience.Insert(obj);
                        }
                    }
                }

                if (model.Industry != null && model.Industry.Length > 0)
                {
                    var findData = _repoCvbuilderIndustry.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (findData != null && findData.Count > 0)
                    {
                        _repoCvbuilderIndustry.DeleteBulk(findData);
                    }
                    foreach (var item in model.Industry)
                    {
                        CvbuilderIndustry obj = new CvbuilderIndustry();
                        obj.CvbuilderId = data.Id;
                        obj.IndustryId = Convert.ToInt32(item);
                        _repoCvbuilderIndustry.Insert(obj);
                    }
                }

                if (model.Technology != null && model.Technology.Length > 0)
                {
                    var findData = _repoCvbuilderTechnology.Query().Get().Where(x => x.CvbuilderId == data.Id).ToList();
                    if (findData != null && findData.Count > 0)
                    {
                        _repoCvbuilderTechnology.DeleteBulk(findData);
                    }
                    foreach (var item in model.Technology)
                    {
                        CvbuilderTechnology obj = new CvbuilderTechnology();
                        obj.CvbuilderId = data.Id;
                        obj.TechnologyId = Convert.ToInt32(item);
                        _repoCvbuilderTechnology.Insert(obj);
                    }
                }
            }

        }
        public Cvbuilder cvBuilderFindById(long Id)
        {
            return _repoCvbuilder.FindById(Id);
        }
        public Cvbuilder cvBuilderFindByUserId(int userId)
        {
            return _repoCvbuilder.Query().Get().Where(x => x.UserId == userId).FirstOrDefault();
        }
        
        public List<Cvbuilder> GetCVBuilderByPaging(out int total, PagingService<Cvbuilder> pagingService)
        {
            return _repoCvbuilder.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }
        public List<CVSpResponse> GetCVBuilderDatasp(out int total, CVSearchRequest SearchFilter,string action)
        {
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_configuration["ConnectionStrings:dsmanagementConnection"]);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
            System.Data.DataSet ds = new System.Data.DataSet();
            cmd = new System.Data.SqlClient.SqlCommand("GetCVData", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pmId", SearchFilter.pm);
            cmd.Parameters.AddWithValue("@TechnologyId", SearchFilter.Technologies);
            cmd.Parameters.AddWithValue("@ExpStage", SearchFilter.SpecType);
            cmd.Parameters.AddWithValue("@DomainId", SearchFilter.Domains);
            cmd.Parameters.AddWithValue("@expId", SearchFilter.ExperienceType);
            cmd.Parameters.AddWithValue("@IsTraning", SearchFilter.TrainingCheck);
            cmd.Parameters.AddWithValue("@PMReview", SearchFilter.PMReviewCheck);
            cmd.Parameters.AddWithValue("@isAnd", SearchFilter.TechnologyrdAnd);
            cmd.Parameters.AddWithValue("@UserId", SearchFilter.UserId);
            cmd.Parameters.AddWithValue("@EmpStatusCheck", SearchFilter.EmpStatusTypeCheck);
            cmd.Parameters.AddWithValue("@BucketProjectList", SearchFilter.BucketProjectList);
            cmd.Parameters.AddWithValue("@PM", SearchFilter.IsPm);
            cmd.Parameters.AddWithValue("@Action", action);
            da = new System.Data.SqlClient.SqlDataAdapter(cmd);
            da.Fill(ds);
            con.Close();
            List<CVSpResponse> cvResponse = new List<CVSpResponse>();
            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
            {
                string domainExperts = dr["DomainId"].ToString();
                List<int> dlist = new List<int>();
                foreach (var dmn in domainExperts.Split(',').ToList())
                {
                    if (!string.IsNullOrEmpty(dmn))
                    {
                        dlist.Add(Convert.ToInt32(dmn));
                    }
                }
                cvResponse.Add(new CVSpResponse
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    UserId = Convert.ToInt32(dr["UserId"]),
                    IsTraining = Convert.ToBoolean(dr["IsTraining"]),
                    IsApproved = Convert.ToBoolean(dr["IsApproved"]),
                    Name = dr["Name"].ToString(),
                    EmailOffice = dr["EmailOffice"].ToString(),
                    MobileNumber = dr["MobileNumber"].ToString(),
                    Experience = dr["Experience"].ToString(),
                    TechTitle = dr["TechTitle"].ToString(),
                    DomainName = dr["DomainName"].ToString(),
                    ExperienceId = Convert.ToInt32(dr["ExperienceId"]),
                    RoleId = Convert.ToInt32(dr["RoleId"]),
                    DomainId = dlist.ToArray()
                });
            }
            total = cvResponse.Count();
            return cvResponse;
        }
        public void UpdateApprovedStatus(Cvbuilder entity)
        {
            _repoCvbuilder.Update(entity);
        }
        public CvBuilderEstimatePrice GetEstimateRoleTechnoloyPrice(int roleId, int estimateRoleExpId, long? technologyParentId)
        {
            return _repoCvBuilderEstimatePrice.Query()
                .Filter(x => x.RoleId == roleId && x.IsActive.Value && x.ExpId == estimateRoleExpId && x.TechId == technologyParentId)
                .Get().FirstOrDefault();

        }
        public void Dispose()
        {
            if (_repoCvbuilder != null)
            {
                _repoCvbuilder.Dispose();
                _repoCvbuilder = null;
            }            
        }


    }
}
