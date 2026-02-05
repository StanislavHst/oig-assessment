using OIG.Assessment.Application.Permissions;
using OIG.Assessment.Domain.Common;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Services;

public class GlobalAdminChecker(
    IUserRepository userRepository,
    IPermissionChecker permissionChecker) : IGlobalAdminChecker
{
    public async Task<bool> IsGlobalAdminAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdWithDetailsAsync(userId, cancellationToken);
        if (user?.Organization == null)
            return false;
        return user.Organization.Name == "System" &&
               await permissionChecker.HasPermissionAsync(userId, Domain.Common.Permissions.AddRootOrganization, cancellationToken);
    }
}
