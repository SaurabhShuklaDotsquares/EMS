using EMS.API.Model;
using EMS.Core;
using EMS.Data;
using EMS.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private readonly IApiKeyService apiKeyService;
        //private IHostingEnvironment _env;
        public BaseApiController(IApiKeyService apiKeyService)
        {           
            this.apiKeyService = apiKeyService;           
        }     

        public ResponseModel<string> AuthorizeRequest()
        {
            ResponseModel<string> response = new ResponseModel<string> { Status = true };
            var res = Request;
            var headers = res.Headers;
            var key = string.Empty;
            string password = string.Empty;

            StringValues apiKey;
            StringValues apiPassword;
            headers.TryGetValue("ApiKey", out apiKey);
            headers.TryGetValue("ApiPassword", out apiPassword);

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                key = apiKey.First();
            }
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                password = apiPassword.First();
            }          
            
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(password))
            {
                response.Status = false;
                response.Code = HttpStatusCode.NotAcceptable;
                response.Errors = new string[] { "Invalid Authentication Parameters." };
            }
            else
            {
                ServiceAuth serviceAuth = apiKeyService.GetApiKey();
                if (serviceAuth == null || serviceAuth.Apikey != key || serviceAuth.ApiPass != password)
                {
                    response.Status = false;
                    response.Code = HttpStatusCode.NotFound;
                    response.Errors = new string[] { "Authentication failed." };
                }
            }
            return response;
        }

        public ResponseStatusModel<string> AuthorizeRequestStatus()
        {
            ResponseStatusModel<string> response = new ResponseStatusModel<string> { Status = true };
            var res = Request;
            var headers = res.Headers;
            var key = string.Empty;
            string password = string.Empty;

            StringValues apiKey;
            StringValues apiPassword;
            headers.TryGetValue("ApiKey", out apiKey);
            headers.TryGetValue("ApiPassword", out apiPassword);

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                key = apiKey.First();
            }
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                password = apiPassword.First();
            }

            if (string.IsNullOrWhiteSpace(key
                ) || string.IsNullOrWhiteSpace(password))
            {
                response.Status = false;
                response.Code = HttpStatusCode.NotAcceptable;
                response.Errors = new string[] { "Invalid Authentication Parameters." };
            }
            else
            {
                ServiceAuth serviceAuth = apiKeyService.GetApiKey();
                if (serviceAuth == null || serviceAuth.Apikey != key || serviceAuth.ApiPass != password)
                {
                    response.Status = false;
                    response.Code = HttpStatusCode.NotFound;
                    response.Errors = new string[] { "Authentication failed." };
                }
            }
            return response;
        }
        //public void WriteLogFile(string description)
        //{
        //    var webRoot = _env.WebRootPath;
        //    var filePath = Path.Combine(webRoot, $"APILog_{DateTime.Today.ToFormatDateString("dd - MM - yyyy")}.txt");
        //    System.IO.File.WriteAllText(filePath, description);          
        //}
    }
}