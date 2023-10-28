using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using SocialStoriesBackend.Mappings;

namespace SocialStoriesBackend.Middleware;

public class NotAuthorizedMiddleware {
    public NotAuthorizedMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        await _next(context);

        if (!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            await context.Response.WriteAsJsonAsync(new ErrorDto{
                Message = new []
                {
                    "You are not authorized to access this resource."
                }
            });
        
        if (!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status403Forbidden)
            await context.Response.WriteAsJsonAsync(new ErrorDto{
                Message = new []
                {
                    "Access this resource is forbidden."
                }
            });
    }
    
    private readonly RequestDelegate _next;
}