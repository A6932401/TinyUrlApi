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
                _requestDelegate.Invoke(context);
            }
            catch (Exception ex) {

                _logger.LogError($"Exception from middleware - {ex.Message} - {(ex.InnerException == null ? "" : ex.InnerException)} - Stack trace {ex.StackTrace}");
            }
        }

    }
}
