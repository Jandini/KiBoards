using Microsoft.AspNetCore.Mvc.Controllers;
using System.Net;
using System.Net.Mime;

namespace KiBoards
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var description = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
                _logger.LogError(ex, "Unhandled exception in {UnhandledExceptionThrownBy}", description?.DisplayName);

                context.Response.ContentType = MediaTypeNames.Text.Plain;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}
