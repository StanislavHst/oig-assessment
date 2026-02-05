using Microsoft.AspNetCore.Http;
using OIG.Assessment.Application.Permissions;

namespace OIG.Assessment.Api.Permissions;

public class PermissionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IPermissionChecker permissionChecker)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await next(context);
            return;
        }

        var permissionAttributes = endpoint.Metadata.GetOrderedMetadata<PermissionAttribute>();
        if (permissionAttributes == null || permissionAttributes.Count == 0)
        {
            await next(context);
            return;
        }

        var userId = ResolveCurrentUserId(context);
        if (userId == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Current user is not specified. Provide user id via 'X-Demo-UserId' header or 'currentUserId' query parameter.");
            return;
        }

        foreach (var attr in permissionAttributes)
        {
            var hasPermission = await permissionChecker.HasPermissionAsync(userId.Value, attr.Permission, context.RequestAborted);
            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Permission '{attr.Permission}' is required.");
                return;
            }
        }

        await next(context);
    }

    private static Guid? ResolveCurrentUserId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Demo-UserId", out var headerValues))
        {
            if (Guid.TryParse(headerValues.FirstOrDefault(), out var headerUserId))
                return headerUserId;
        }

        var queryValue = context.Request.Query["currentUserId"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(queryValue) && Guid.TryParse(queryValue, out var queryUserId))
            return queryUserId;

        return null;
    }
}

