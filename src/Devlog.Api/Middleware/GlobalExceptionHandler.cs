using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using DevLog.Domain.Entities;
using DevLog.Shared.Exceptions;
using DevLog.Shared.Responses;

namespace DevLog.Api.Middleware;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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
        catch(BadRequestException ex)
        {
            _logger.LogWarning("Bad Request on {Method} {Path} : {Message}", 
            context.Request.Method,
            context.Request.Path,
            ex.Message);

            await WriteErrorResponse(context,HttpStatusCode.BadRequest,ex.Message);
        }
        catch(NotFoundException ex)
        {
            _logger.LogWarning("Not Found on {Method} {Path} : {Message}", 
            context.Request.Method,
            context.Request.Path,
            ex.Message);

            await WriteErrorResponse(context,HttpStatusCode.NotFound,ex.Message);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception {Method} {Path}", 
            context.Request.Method,
            context.Request.Path);

            await WriteErrorResponse(context,HttpStatusCode.InternalServerError,"An unexpected error occurred.");
        }
    }

    //helper method
    public static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        //set the response content type to JSON
        context.Response.ContentType = "application/json";

        //set the http status code
        context.Response.StatusCode = (int)statusCode;

        //build the response 
        var response = ApiResponse<object>.Failure(message, new List<string> {message}); //wrap message in a list

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

}