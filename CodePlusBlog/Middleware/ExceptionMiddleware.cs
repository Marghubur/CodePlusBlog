using CodePlusBlog.Service;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mime;

namespace CodePlusBlog.Middleware
{
    public class ExceptionMiddleware: IMiddleware
    {
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {

                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = _environment.IsDevelopment() ?
                new ApiResponse(ex.StackTrace.ToString(), context.Response.StatusCode, ex.Message)
                : new ApiResponse(ex.Message, context.Response.StatusCode);

            var json = JsonConvert.SerializeObject(response);
            await context.Response.WriteAsync(json);
        }
    }
}
