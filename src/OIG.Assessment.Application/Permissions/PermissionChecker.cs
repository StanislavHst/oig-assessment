using OIG.Assessment.Domain.Repositories;
using OIG.Assessment.Domain.Services;

namespace OIG.Assessment.Application.Permissions;

public class PermissionChecker(
    IUserRepository userRepository,
    IPermissionService permissionService) : IPermissionChecker
{
    public async Task<IReadOnlyList<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdWithDetailsAsync(userId, cancellationToken);
        if (user == null)
            return Array.Empty<string>();

        var permissions = permissionService.GetUserPermissions(user);
        return permissions.ToList();
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default)
    {
        var permissions = await GetPermissionsAsync(userId, cancellationToken);
        return permissions.Contains(permission);
    }
}

