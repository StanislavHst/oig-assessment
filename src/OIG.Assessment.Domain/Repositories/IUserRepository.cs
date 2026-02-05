using OIG.Assessment.Domain.Entities;

namespace OIG.Assessment.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByOrganizationIdWithDescendantsAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
