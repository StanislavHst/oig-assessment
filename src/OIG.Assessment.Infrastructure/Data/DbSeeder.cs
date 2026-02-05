using Microsoft.EntityFrameworkCore;
using OIG.Assessment.Domain.Common;
using OIG.Assessment.Domain.Entities;

namespace OIG.Assessment.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Organizations.AnyAsync(cancellationToken))
            return;

        var systemOrg = new Organization { Name = "System" };
        
        var rootOrg = new Organization { Name = "Head Office", Parent = systemOrg };
        var salesOrg = new Organization { Name = "Sales", Parent = rootOrg };
        var itOrg = new Organization { Name = "IT", Parent = rootOrg };

        var allPermissions = Permissions.GetAll().ToList();
        
        var headOfficePermissions = allPermissions.Where(p => p != Permissions.AddRootOrganization).ToList();

        var systemAdminRole = new Role
        {
            Name = "System Admin",
            Organization = systemOrg,
            Permissions = allPermissions.ToList()
        };

        var headOfficeAdminRole = new Role
        {
            Name = "Head of Office Admin",
            Organization = rootOrg,
            Permissions = headOfficePermissions
        };

        var salesManagerRole = new Role
        {
            Name = "Sales Manager",
            Organization = salesOrg,
            Permissions = new List<string> { Permissions.ViewUserList, Permissions.AddUser, Permissions.EditUser, Permissions.ViewRolesList }
        };

        var viewerRole = new Role
        {
            Name = "Viewer",
            Organization = rootOrg,
            Permissions = new List<string> { Permissions.ViewUserList, Permissions.ViewRolesList }
        };

        var systemAdminUser = new User
        {
            Name = "System Admin",
            Email = "system-admin@example.com",
            Organization = systemOrg,
            Roles = new List<Role> { systemAdminRole }
        };

        var headOfficeUser = new User
        {
            Name = "Head of Office",
            Email = "head.office@example.com",
            Organization = rootOrg,
            Roles = new List<Role> { headOfficeAdminRole }
        };

        var salesUser = new User
        {
            Name = "Sales User",
            Email = "sales@example.com",
            Organization = salesOrg,
            Roles = new List<Role> { salesManagerRole }
        };

        var viewerUser = new User
        {
            Name = "Viewer User",
            Email = "viewer@example.com",
            Organization = itOrg,
            Roles = new List<Role> { viewerRole }
        };

        await context.Organizations.AddRangeAsync(new[] { systemOrg, rootOrg, salesOrg, itOrg }, cancellationToken);
        await context.Roles.AddRangeAsync(new[] { systemAdminRole, headOfficeAdminRole, salesManagerRole, viewerRole }, cancellationToken);
        await context.Users.AddRangeAsync(new[] { systemAdminUser, headOfficeUser, salesUser, viewerUser }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}

