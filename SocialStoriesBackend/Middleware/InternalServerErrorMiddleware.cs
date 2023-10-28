using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using SocialStoriesBackend.Mappings;

namespace SocialStoriesBackend.Middleware;

public class InternalServerErrorMiddleware
{
    public InternalServerErrorMiddleware(RequestDelegate next, ILogger<InternalServerErrorMiddleware> logger)
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
        catch (Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "[InternalServerErrorMiddleware] {url}", context.Request.GetDisplayUrl());

                // legacy view error handler
                if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/api/legacy/"))
                {
                    await context.Response.WriteAsJsonAsync(new ErrorDto
                    {
                        Message = new []
                        {
                            "Something went wrong and we were unable to complete the call. Our developers have been notified.<br/>"
                        }
                    });

                    return;
                }

                var error = "An internal server error has occurred, our developers have been notified.";
                if (Debugger.IsAttached)
                {
                    var errorResponse = new ErrorDto
                    {
                        Message = new[]
                        {
                            error
                        }
                    };

                    errorResponse.Message = errorResponse.Message.Append(exception.ToString()).ToArray();
                    await context.Response.WriteAsJsonAsync(errorResponse);
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(new ErrorDto
                    {
                        Message = new[]
                        {
                            error
                        }
                    });
                }
            }
        }
    }

    private readonly RequestDelegate _next;
    private readonly ILogger<InternalServerErrorMiddleware> _logger;
}