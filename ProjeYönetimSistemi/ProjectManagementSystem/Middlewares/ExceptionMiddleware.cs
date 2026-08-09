using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProjectManagementSystem;
using ProjectManagementSystem.Models;
using System.Net;

namespace ProjectManagementSystem.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }

            catch (Exception ex)
            {
                _logger.LogError($"Bir hata oluştu: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType= "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if(exception is UnauthorizedAccessException)
            {
                context.Response.StatusCode=(int)HttpStatusCode.Unauthorized;
            }

            else if (exception is ArgumentException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            var response = new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            };

            await context.Response.WriteAsync(response.ToString());
        }
    }
}
