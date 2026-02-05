using OIG.Assessment.Domain.Entities;

namespace OIG.Assessment.Domain.Services;

public interface IPermissionService
{
    HashSet<string> GetUserPermissions(User user);
    
    IEnumerable<Role> GetAvailableRolesForUser(User user, IEnumerable<Role> allRoles);

    Task<IEnumerable<Role>> GetAssignableRolesAsync(User currentUser, IEnumerable<Role> allRoles, CancellationToken cancellationToken = default);
}
