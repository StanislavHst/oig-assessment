using OIG.Assessment.Domain.Entities;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Domain.Services;

public class PermissionService : IPermissionService
{
    private readonly IOrganizationRepository _organizationRepository;

    public PermissionService(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }
    public HashSet<string> GetUserPermissions(User user)
    {
        var permissions = new HashSet<string>();
        foreach (var role in user.Roles)
        {
            foreach (var permission in role.Permissions)
                permissions.Add(permission);
        }
        return permissions;
    }

    public IEnumerable<Role> GetAvailableRolesForUser(User user, IEnumerable<Role> allRoles)
    {
        var userOrganization = user.Organization;
        var availableOrganizationIds = GetAvailableOrganizationIds(userOrganization);
        return allRoles.Where(role => availableOrganizationIds.Contains(role.OrganizationId));
    }

    private HashSet<Guid> GetAvailableOrganizationIds(Organization organization)
    {
        var organizationIds = new HashSet<Guid> { organization.Id };
        var current = organization.Parent;
        while (current != null)
        {
            organizationIds.Add(current.Id);
            current = current.Parent;
        }
        return organizationIds;
    }

    public async Task<IEnumerable<Role>> GetAssignableRolesAsync(User currentUser, IEnumerable<Role> allRoles, CancellationToken cancellationToken = default)
    {
        if (currentUser == null || currentUser.Organization == null)
            return Enumerable.Empty<Role>();

        var assignableOrganizationIds = new HashSet<Guid> { currentUser.Organization.Id };
        var descendants = await _organizationRepository.GetDescendantsAsync(currentUser.Organization.Id, cancellationToken);
        foreach (var descendant in descendants)
        {
            assignableOrganizationIds.Add(descendant.Id);
        }

        return allRoles.Where(role => assignableOrganizationIds.Contains(role.OrganizationId));
    }
}
