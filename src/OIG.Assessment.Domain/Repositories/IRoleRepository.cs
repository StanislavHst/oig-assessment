using OIG.Assessment.Domain.Entities;

namespace OIG.Assessment.Domain.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetByOrganizationIdsAsync(IEnumerable<Guid> organizationIds, CancellationToken cancellationToken = default);
}
