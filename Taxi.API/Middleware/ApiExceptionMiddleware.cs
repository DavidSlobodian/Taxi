using System.Text.Json;
using Taxi.App.Models;
using Taxi.Domain.Exceptions;
using Taxi.Domain.Responses;

namespace Taxi.Api.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    //private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(
        RequestDelegate next)
        //ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        //_logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                //_logger.LogError(ex,"Exception occurred after the response started.");

                throw;
            }

            context.Response.Clear();

            int statusCode;
            string message;

            if (ex is ApiException apiException)
            {
                statusCode = apiException.StatusCode;
                message = apiException.Message;

                //_logger.LogWarning("API error {StatusCode}: {Message}",statusCode,message);
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                message = "An unexpected error occurred.";

                //_logger.LogError(ex, "Unhandled exception occurred.");
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail(
                statusCode,
                message);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}