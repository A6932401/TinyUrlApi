using Microsoft.AspNetCore.Http;
using System.Net;
using TinyUrlApp.Model;

namespace TinyUrlApp.Handler
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate requestDelegate,ILogger<ExceptionMiddleware> logger) {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
              await _requestDelegate.Invoke(context);
            }
            catch (Exception ex) {

                _logger.LogError($"Exception from middleware - {ex.Message} - {(ex.InnerException == null ? "" : ex.InnerException)} - Stack trace {ex.StackTrace}");

                HandleExceptionAsync(context,ex);

            }
}

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new ResponceModel<string>
            {
                status = "error",
                message = ex switch 
                {
                    ArgumentException => ex.Message,
                    KeyNotFoundException => ex.Message,
                    _ => "An unexpected error occurred. Contact your admin."
                }
            };

            await context.Response.WriteAsJsonAsync(response);
        }


    }
}
