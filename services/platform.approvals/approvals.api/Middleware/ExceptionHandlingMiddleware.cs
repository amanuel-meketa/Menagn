using System.Net;
using System.Text.Json;
using approvals.api.Model;
using approvals.application.Common.Exceptions;
using FluentValidation;

namespace approvals.api.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found");
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex);
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict");
                await HandleExceptionAsync(context, HttpStatusCode.Conflict, ex);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request");
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed");
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, HttpStatusCode code, Exception ex)
        {
            context.Response.StatusCode = (int)code;
            context.Response.ContentType = "application/json";

            var response = new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.Message,
                Source = ex.Source
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
