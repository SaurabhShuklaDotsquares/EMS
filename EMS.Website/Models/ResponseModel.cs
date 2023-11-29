using System.Net;

namespace EMS.Web.Modals
{

    public class ResponseModel<T>
    {
        public ResponseModel()
        {
            Code = HttpStatusCode.OK;
        }
        public bool Status { get; set; }      
        public string Message { get; set; }
        public T Data { get; set; }
        public HttpStatusCode Code { get; set; }    
        public string[] Errors { get; set; }
    }
  
}