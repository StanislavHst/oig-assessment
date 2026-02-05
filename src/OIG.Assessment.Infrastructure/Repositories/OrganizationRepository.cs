using Microsoft.EntityFrameworkCore;
using OIG.Assessment.Domain.Entities;
using OIG.Assessment.Domain.Repositories;
using OIG.Assessment.Infrastructure.Data;

namespace OIG.Assessment.Infrastructure.Repositories;

public class OrganizationRepository(ApplicationDbContext context) : Repository<Organization>(context), IOrganizationRepository
{
    public async Task<Organization?> GetByIdWithChildrenAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(o => o.Parent)
            .Include(o => o.Children)
            .Include(o => o.Roles)
            .Include(o => o.Users)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<IEnumerable<Organization>> GetRootOrganizationsAsync(CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(o => o.Children)
            .Where(o => o.ParentId == null)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Organization>> GetVisibleRootOrganizationsAsync(CancellationToken cancellationToken = default)
    {
        var systemOrg = await DbSet.FirstOrDefaultAsync(o => o.Name == "System", cancellationToken);
        if (systemOrg == null)
            return Array.Empty<Organization>();
        return await DbSet
            .Include(o => o.Children)
            .Where(o => o.ParentId == systemOrg.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Organization?> GetSystemOrganizationAsync(CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(o => o.Name == "System", cancellationToken);

    public async Task<IEnumerable<Organization>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(o => o.Children)
            .Where(o => o.ParentId == parentId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Organization>> GetAncestorsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var ancestors = new List<Organization>();
        var organization = await DbSet
            .Include(o => o.Parent)
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);
        if (organization == null)
            return ancestors;
        var current = organization.Parent;
        while (current != null)
        {
            ancestors.Add(current);
            current = await DbSet
                .Include(o => o.Parent)
                .FirstOrDefaultAsync(o => o.Id == current.Id, cancellationToken);
            current = current?.Parent;
        }
        return ancestors;
    }

    public async Task<IEnumerable<Organization>> GetDescendantsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var descendants = new List<Organization>();
        await CollectDescendantsAsync(organizationId, descendants, cancellationToken);
        return descendants;
    }

    private async Task CollectDescendantsAsync(Guid parentId, List<Organization> descendants, CancellationToken cancellationToken)
    {
        var children = await DbSet
            .Include(o => o.Children)
            .Where(o => o.ParentId == parentId)
            .ToListAsync(cancellationToken);
        foreach (var child in children)
        {
            descendants.Add(child);
            await CollectDescendantsAsync(child.Id, descendants, cancellationToken);
        }
    }
}
