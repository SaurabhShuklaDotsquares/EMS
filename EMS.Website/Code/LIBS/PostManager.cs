using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace EMS.Web.Code.LIBS
{
    public class PostManager
    {
        private readonly WebRequest request;
        public PostManager(string ApiUrl)
        {
            request = WebRequest.Create(ApiUrl);
        }

        public void AddHeader(string key, string value)
        {
            request.Headers.Add(key, value);
        }

        public Tout PostData<Tin, Tout>(Tin data)
        {
            request.Method = "POST";
            var postData = JsonConvert.SerializeObject(data);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<Tout>(responseFromServer);
            reader.Close();
            dataStream.Close();
            response.Close();
            return result;
        }
    }
}