using Microsoft.EntityFrameworkCore;
using OIG.Assessment.Domain.Entities;
using OIG.Assessment.Domain.Repositories;
using OIG.Assessment.Infrastructure.Data;

namespace OIG.Assessment.Infrastructure.Repositories;

public class RoleRepository(ApplicationDbContext context) : Repository<Role>(context), IRoleRepository
{
    public async Task<Role?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(r => r.Organization)
                .ThenInclude(o => o.Parent)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IEnumerable<Role>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(r => r.Organization)
                .ThenInclude(o => o.Parent)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Role>> GetByOrganizationIdsAsync(IEnumerable<Guid> organizationIds, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(r => r.Organization)
            .Where(r => organizationIds.Contains(r.OrganizationId))
            .ToListAsync(cancellationToken);
}
