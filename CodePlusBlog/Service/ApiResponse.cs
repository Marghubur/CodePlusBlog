using System.Net;

namespace CodePlusBlog.Service
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Token { get; set; }
        public string StatusMessage { get; set; }
        public dynamic ResponseBody { get; set; }
        public ApiResponse(dynamic data, int httpStatusCode, string message = null, string token = null)
        {
            ResponseBody = data;
            StatusCode = httpStatusCode;
            Token = token;
            StatusMessage = message;
        }

        public ApiResponse(dynamic data)
        {
            ResponseBody = data;
            StatusCode = (int)HttpStatusCode.OK;
            Token = null;
            StatusMessage = "Successfull";
        }

        public ApiResponse()
        {
        }
    }
}
