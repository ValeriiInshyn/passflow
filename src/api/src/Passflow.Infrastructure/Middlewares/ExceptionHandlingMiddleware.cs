using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passflow.Contracts.Exceptions;
using Serilog;
using Serilog.Events;

namespace Passflow.Infrastructure.Middlewares;

/// <summary>
///     The exception handling middleware class
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    /// <summary>
    ///     The next - request delegate
    /// </summary>
    private readonly RequestDelegate _next;
    /// <summary>
    ///     The logger
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExceptionHandlingMiddleware" /> class
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="next">The next</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    ///     Invokes the context. (Invokes when request came on endpoint)
    /// </summary>
    /// <param name="context">The context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            ProblemDetails problemDetails = new()
            {
                Status = ex.StatusCode,
                Type = ex.GetType().Name,
                Title = ex.Message,
                Detail = ex.Description
            };
            _logger.Write(ex.GetLevel(), ex, JsonSerializer.Serialize(problemDetails), ex.Description);
            await HandleExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    ///     Handles the exception using the specified context
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="exception">The exception</param>
    private static Task HandleExceptionAsync(HttpContext context, ApiException ex)
    {
        context.Response.StatusCode = ex.StatusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(ex.ToString()); ;
    }

    /// <summary>
    ///     Handles the exception using the specified context
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="exception">The exception</param>
    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(
            new ApiException(ex.Message, context.Response.StatusCode, LogEventLevel.Fatal).ToString());
    }
}