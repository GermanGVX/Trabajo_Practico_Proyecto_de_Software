using System.Net;
using System.Text.Json;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Trabajo_Practoco_Proyecto_de_Software.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {

            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            var detail = "An internal server error occurred.";

            // Mapeo centralizado de excepciones a HTTP Status
            switch (ex)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    detail = ex.Message;
                    break;
                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    detail = ex.Message;
                    break;
                case ConcurrencyException:
                case ConflictException:
                    statusCode = HttpStatusCode.Conflict;
                    detail = ex.Message;
                    break;
            }


            var problem = new
            {
                type = $"https://httpstatuses.com/{(int)statusCode}",
                title = WebUtility.HtmlEncode(statusCode.ToString()),
                status = (int)statusCode,
                detail = detail,
                instance = context.Request.Path
            };

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
