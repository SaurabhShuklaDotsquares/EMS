using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using EMS.Data;
using EMS.Dto;
using EMS.Web.Controllers;
using EMS.Core;
using EMS.Web.Code.LIBS;
using EMS.Web.Code.Attributes;
using DataTables.AspNet.Core;

namespace EMS.Website.Controllers
{
    public class SelfDeclarationController : BaseController
    {
        private readonly ISelfDeclarationService selfDeclarationService;
        private readonly IUserLoginService userLoginService;
        public SelfDeclarationController(ISelfDeclarationService _selfDeclarationService, IUserLoginService _userLoginService)
        {
            this.selfDeclarationService = _selfDeclarationService;
            this.userLoginService = _userLoginService;
        }
        #region SelfDeclaration Index

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, string txtSearch, string drpInJpr,
            string drpHasDiseaseSymptoms)
        {
            var pagingServices = new PagingService<SelfDeclaration>(request.Start, request.Length);
            var expr = PredicateBuilder.True<SelfDeclaration>();


            bool isHRUser = CurrentUser.RoleId == (int)Enums.UserRoles.HRBP;
            //int currentUserId = CurrentUser.Uid;

            //if (isHRUser)
            //{
            //    expr = expr.And(x => x.Uid == currentUserId || x.U.PMUid == currentUserId);
            //}
            //else
            //{
            //    expr = expr.And(x => x.Uid == currentUserId);
            //}

            //if (request.Search.Value.HasValue())
            //{
            //    string searchValue = request.Search.Value.Trim().ToLower();
            //    expr = expr.And(x => x.U != null && x.U.Name.ToLower().Contains(searchValue));
            //}


            if (txtSearch.HasValue())
            {
                string searchValue = txtSearch.Trim().ToLower();
                expr = expr.And(x => x.U != null && x.U.Name.ToLower().Contains(searchValue));
            }

            if (drpInJpr.HasValue() && !drpInJpr.Equals("0") )
            {
                if (drpInJpr.Equals("1"))
                {
                    expr = expr.And(x => x.RecentlyInJaipur == true);
                }
                else
                {
                    expr = expr.And(x => x.RecentlyInJaipur == false);
                }
            }

            if (drpHasDiseaseSymptoms.HasValue() && !drpHasDiseaseSymptoms.Equals("0"))
            {
                if (drpHasDiseaseSymptoms.Equals("1"))
                {
                    expr = expr.And(x => x.HasDiseaseSymptoms == true);
                }
                else
                {
                    expr = expr.And(x => x.HasDiseaseSymptoms == false);
                }
            }


            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.Name).ThenByDescending(c=>c.AddDate);
            };

            int totalCount = 0;

            var response = selfDeclarationService.GetSelfDeclarationByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.Id,
                //Name = (r.U?.Name)!=null?$"{r.U?.Name} {(!string.IsNullOrWhiteSpace(r.EmpCode)?(r.EmpCode):"")}":r.EmpCode,
                Name = (string.IsNullOrEmpty(r.EmpCode)) ? r.U?.Name : ((r.U?.Name) != null ? r.U.Name : "") + " " + "( " + r.EmpCode + ")",
                r.MobileNumber,
                RecentlyInJaipur = r.RecentlyInJaipur ? "Yes" : "No",
                HasDiseaseSymptoms = r.HasDiseaseSymptoms ? "Yes" : "No",
                UserId = r.Uid,
                AddedDate = r.AddDate.ToFormatDateString("MMM dd, yyyy"),
                AllowEditDelete = isHRUser
            }));

        }

        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Add(int? uid)
        {
            var User = CurrentUser.RoleId == (int)Enums.UserRoles.HRBP && uid != null ? uid : CurrentUser.Uid;

            SelfDeclaration SelfDeclarationModel = new SelfDeclaration();
            SelfDeclarationDto selfDeclarationDto = new SelfDeclarationDto();
            try
            {
                SelfDeclarationModel = selfDeclarationService.GetSelfDeclarationByUid((int)User);
                if (SelfDeclarationModel != null)
                {
                    selfDeclarationDto.Id = SelfDeclarationModel.Id;
                    selfDeclarationDto.Uid = SelfDeclarationModel.Uid;
                    selfDeclarationDto.Name = SelfDeclarationModel.Name;
                    selfDeclarationDto.Dob = SelfDeclarationModel.Dob.ToFormatDateString("dd/MM/yyyy");
                    selfDeclarationDto.JoiningDate = SelfDeclarationModel.JoiningDate.ToFormatDateString("dd/MM/yyyy");
                    selfDeclarationDto.EmpCode = !string.IsNullOrWhiteSpace(SelfDeclarationModel.EmpCode) ? SelfDeclarationModel.EmpCode : "";
                    selfDeclarationDto.MobileNumber = SelfDeclarationModel.MobileNumber;
                    selfDeclarationDto.EmailPersonal = SelfDeclarationModel.EmailPersonal;
                    selfDeclarationDto.Address = SelfDeclarationModel.Address;
                    selfDeclarationDto.LocalAddress = SelfDeclarationModel.LocalAddress;
                    selfDeclarationDto.RecentlyInJaipur = SelfDeclarationModel.RecentlyInJaipur;
                    selfDeclarationDto.Location = SelfDeclarationModel.Location;
                    selfDeclarationDto.Purpose = SelfDeclarationModel.Purpose;
                    selfDeclarationDto.HasDiseaseSymptoms = SelfDeclarationModel.HasDiseaseSymptoms;
                    selfDeclarationDto.SymptomsStartDate = SelfDeclarationModel.SymptomsStartDate.ToFormatDateString("dd/MM/yyyy"); ;
                    selfDeclarationDto.SymptomsEndDate = SelfDeclarationModel.SymptomsEndDate.ToFormatDateString("dd/MM/yyyy"); ;
                    selfDeclarationDto.DeclarationName = SelfDeclarationModel.Name;


                    selfDeclarationDto.HasCoughSymptoms = SelfDeclarationModel.HasCoughSymptoms ?? false;
                    selfDeclarationDto.HasFeverSymptoms = SelfDeclarationModel.HasFeverSymptoms ?? false;
                    selfDeclarationDto.HasBreathingSymptoms = SelfDeclarationModel.HasBreathingSymptoms ?? false;
                    selfDeclarationDto.HasSmellAndTasteSymptoms = SelfDeclarationModel.HasSmellAndTasteSymptoms ?? false;
                    selfDeclarationDto.HasDiabetesProblem = SelfDeclarationModel.HasDiabetesProblem ?? false;
                    selfDeclarationDto.HasHypertensionProblem = SelfDeclarationModel.HasHypertensionProblem ?? false;
                    selfDeclarationDto.HasLungProblem = SelfDeclarationModel.HasLungProblem ?? false;
                    selfDeclarationDto.HasHeartProblem = SelfDeclarationModel.HasHeartProblem ?? false;
                    selfDeclarationDto.HasKidneyProblem = SelfDeclarationModel.HasKidneyProblem ?? false;
                    selfDeclarationDto.HasTraveledInternationally = SelfDeclarationModel.HasTraveledInternationally ?? false;
                }
                else
                {
                    UserLogin UserModelDB = userLoginService.GetUserInfoByID(CurrentUser.Uid);
                    if (UserModelDB != null)

                    {
                        selfDeclarationDto.Uid = UserModelDB.Uid;
                        selfDeclarationDto.Name = UserModelDB.Name;
                        selfDeclarationDto.Dob = UserModelDB.DOB.ToFormatDateString("dd/MM/yyyy");
                        selfDeclarationDto.JoiningDate = UserModelDB.JoinedDate.ToFormatDateString("dd/MM/yyyy");
                        selfDeclarationDto.EmpCode = !string.IsNullOrWhiteSpace(UserModelDB.EmpCode) ? UserModelDB.EmpCode : "";
                        selfDeclarationDto.MobileNumber = UserModelDB.MobileNumber;
                        selfDeclarationDto.EmailPersonal = UserModelDB.EmailPersonal;
                        selfDeclarationDto.Address = UserModelDB.Address;
                        selfDeclarationDto.DeclarationName = UserModelDB.Name;

                        //selfDeclarationDto.LocalAddress = UserModelDB.LocalAddress;
                        //selfDeclarationDto.RecentlyInJaipur = UserModelDB.RecentlyInJaipur;
                        //selfDeclarationDto.Location = UserModelDB.Location;
                        //selfDeclarationDto.Purpose = UserModelDB.Purpose;
                        //    selfDeclarationDto.HasDiseaseSymptoms = UserModelDB.HasDiseaseSymptoms;
                        //    selfDeclarationDto.SymptomsStartDate = UserModelDB.SymptomsStartDate.ToFormatDateString("dd/MM/yyyy"); ;
                        //    selfDeclarationDto.SymptomsEndDate = UserModelDB.SymptomsEndDate.ToFormatDateString("dd/MM/yyyy"); ;
                        //    selfDeclarationDto.DeclarationName = UserModelDB.DeclarationName;
                        //}

                    }
                    else
                    {
                        return MessagePartialView("Unable to find record");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("", "Error occured", false);
            }
            return View(selfDeclarationDto);
        }


        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Add(SelfDeclarationDto model)
        {
            if(ModelState.IsValid)
            {
                    model.Uid = CurrentUser.Uid;
                    model.Ip = GeneralMethods.Getip();
                    var result=selfDeclarationService.Save(model);
                if(result.Id>0)
                {
                    return NewtonSoftJsonResult(
                        new RequestOutcome<string> { Message = "Record has been updated successfully.", IsSuccess = true});
                }
                return NewtonSoftJsonResult(
                        new RequestOutcome<string> { Message = "Unable to save.", IsSuccess = false });
            }
            return CreateModelStateErrors();
        }
    }
}