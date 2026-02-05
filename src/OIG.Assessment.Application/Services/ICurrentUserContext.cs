namespace OIG.Assessment.Application.Services;

public interface ICurrentUserContext
{
    Guid? GetCurrentUserId();
}
