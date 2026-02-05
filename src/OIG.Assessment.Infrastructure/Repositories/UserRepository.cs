using Microsoft.EntityFrameworkCore;
using OIG.Assessment.Domain.Entities;
using OIG.Assessment.Domain.Repositories;
using OIG.Assessment.Infrastructure.Data;

namespace OIG.Assessment.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(u => u.Organization)
                .ThenInclude(o => o.Parent)
            .Include(u => u.Organization)
                .ThenInclude(o => o.Children)
            .Include(u => u.Roles)
                .ThenInclude(r => r.Organization)
                    .ThenInclude(o => o.Parent)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(u => u.Organization)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<IEnumerable<User>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(u => u.Organization)
            .Include(u => u.Roles)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<User>> GetByOrganizationIdWithDescendantsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var organizationIds = await GetDescendantOrganizationIdsAsync(organizationId, cancellationToken);
        organizationIds.Add(organizationId);
        return await DbSet
            .Include(u => u.Organization)
            .Include(u => u.Roles)
            .Where(u => organizationIds.Contains(u.OrganizationId))
            .ToListAsync(cancellationToken);
    }

    private async Task<HashSet<Guid>> GetDescendantOrganizationIdsAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var organizationIds = new HashSet<Guid>();
        var organizations = await Context.Organizations
            .Where(o => o.ParentId == organizationId)
            .ToListAsync(cancellationToken);
        foreach (var org in organizations)
        {
            organizationIds.Add(org.Id);
            var descendants = await GetDescendantOrganizationIdsAsync(org.Id, cancellationToken);
            foreach (var descendantId in descendants)
                organizationIds.Add(descendantId);
        }
        return organizationIds;
    }
}
