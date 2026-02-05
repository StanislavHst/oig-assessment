using OIG.Assessment.Application.Services;

namespace OIG.Assessment.Api.Services;

public class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public Guid? GetCurrentUserId()
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null)
            return null;

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
