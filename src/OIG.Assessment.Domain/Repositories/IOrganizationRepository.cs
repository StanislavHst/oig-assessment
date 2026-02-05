using OIG.Assessment.Domain.Entities;

namespace OIG.Assessment.Domain.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetByIdWithChildrenAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Organization>> GetRootOrganizationsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Organization>> GetVisibleRootOrganizationsAsync(CancellationToken cancellationToken = default);
    
    Task<Organization?> GetSystemOrganizationAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Organization>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Organization>> GetAncestorsAsync(Guid organizationId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Organization>> GetDescendantsAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
