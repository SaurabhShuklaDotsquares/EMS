using EMS.API.Model;
using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : BaseApiController
    {
        #region Reference Variables
        private readonly IRoleService roleService;
        private readonly IKRAServices kRAServices;
        #endregion

        #region Constructor
        public DesignationController(IApiKeyService _apiKeyService, IRoleService _roleService, IKRAServices _kRAServices) : base(_apiKeyService)
        {
            roleService = _roleService;
            kRAServices = _kRAServices;
        }
        #endregion

        [Route("~/Designation/hello")]
        [HttpGet]
        public ResponseModel<string> hello()
        {
            return new ResponseModel<string>
            {
                Status = true,
                Message = "True"
            };
        }

        [Route("~/Designation/GetRoleCategories")]
        [HttpGet]
        public IActionResult GetRoleCategories()
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseModel<List<RoleCategoryResponseModel>> response = new ResponseModel<List<RoleCategoryResponseModel>>();
            authResponse = AuthorizeRequestStatus();
            if (authResponse.Status)
            {
                try
                {
                    var roleCategories = roleService.GetActiveRoleCategory();
                    if (roleCategories != null)
                    {
                        List<RoleCategoryResponseModel> data = new List<RoleCategoryResponseModel>();
                        foreach (var item in roleCategories)
                        {
                            RoleCategoryResponseModel obj = new RoleCategoryResponseModel();
                            obj.Id = item.Id;
                            obj.Name = item.Name;                            
                            data.Add(obj);
                        }
                        response.Status = true;
                        response.Message = "OK";
                        response.Data = data;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Role category does not exist.";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = "Error";
                }
            }
            else
            {
                response.Status = false;
                response.Message = "Invalid authentication found";
                response.Code = HttpStatusCode.NetworkAuthenticationRequired;
                response.Errors = new string[] { "Invalid authentication found" };
                return Ok(response);
            }


            return Ok(response);
        }

        [Route("~/Designation/GetRoleByCategory")]
        [HttpGet]
        public IActionResult GetRoleByCategory(int RoleCateGoryId)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseModel<List<RoleResponseModel>> response = new ResponseModel<List<RoleResponseModel>>();
            authResponse = AuthorizeRequestStatus();
            if (authResponse.Status)
            {
                try
                {
                    var roles = roleService.GetRolesByRoleCategoyId(RoleCateGoryId);
                    if (roles != null)
                    {
                        List<RoleResponseModel> data = new List<RoleResponseModel>();
                        foreach (var item in roles)
                        {
                            RoleResponseModel obj = new RoleResponseModel();
                            obj.RoleId = item.RoleId;
                            obj.RoleName = item.RoleName;
                            obj.RoleCategoryId = item.RoleCategoryId;
                            data.Add(obj);
                        }
                        response.Status = true;
                        response.Message = "OK";
                        response.Data = data;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Role does not exist.";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = "Error";
                }
            }
            else
            {
                response.Status = false;
                response.Message = "Invalid authentication found";
                response.Code = HttpStatusCode.NetworkAuthenticationRequired;
                response.Errors = new string[] { "Invalid authentication found" };
                return Ok(response);
            }


            return Ok(response);
        }

        [Route("~/Designation/GetDesignationByRole")]
        [HttpGet]
        public IActionResult GetDesignationByRole(int RoleId)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseModel<List<DesignationResponseModel>> response = new ResponseModel<List<DesignationResponseModel>>();
            authResponse = AuthorizeRequestStatus();
            if (authResponse.Status)
            {
                try
                {
                    var designation = roleService.GetDesignationByRoleId(RoleId);
                    if (designation != null)
                    {
                        List<DesignationResponseModel> data = new List<DesignationResponseModel>();
                        foreach (var item in designation)
                        {
                            DesignationResponseModel obj = new DesignationResponseModel();
                            obj.Id = item.Id;
                            obj.Name = item.Name;
                            obj.RoleId = item.RoleId;
                            obj.Experience = item.Experience;
                            data.Add(obj);
                        }
                        response.Status = true;
                        response.Message = "OK";
                        response.Data = data;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Designation does not exist.";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = "Error";
                }
            }
            else
            {
                response.Status = false;
                response.Message = "Invalid authentication found";
                response.Code = HttpStatusCode.NetworkAuthenticationRequired;
                response.Errors = new string[] { "Invalid authentication found" };
                return Ok(response);
            }


            return Ok(response);
        }

        [Route("~/Designation/GetDesignationKRA")]
        [HttpGet]
        public IActionResult GetDesignationKRA(int DesignationId)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseModel<List<KRAResponseModel>> response = new ResponseModel<List<KRAResponseModel>>();
            authResponse = AuthorizeRequestStatus();
            if (authResponse.Status)
            {
                try
                {
                    var kraData = kRAServices.GetKradataByDesgnationId(DesignationId);
                    if (kraData != null)
                    {
                        List<KRAResponseModel> data = new List<KRAResponseModel>();
                        foreach (var item in kraData)
                        {
                            KRAResponseModel obj = new KRAResponseModel();
                            obj.Title = item.Title;
                            obj.Designation = item.Designation.Name;
                            data.Add(obj);
                        }
                        response.Status = true;
                        response.Message = "OK";
                        response.Data = data;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "KRA does not exist.";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = "Error";
                }
            }
            else
            {
                response.Status = false;
                response.Message = "Invalid authentication found";
                response.Code = HttpStatusCode.NetworkAuthenticationRequired;
                response.Errors = new string[] { "Invalid authentication found" };
                return Ok(response);
            }


            return Ok(response);
        }
    }
}
