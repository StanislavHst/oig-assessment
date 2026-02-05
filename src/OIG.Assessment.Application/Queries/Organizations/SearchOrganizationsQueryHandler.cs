using MediatR;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Queries.Organizations;

public class SearchOrganizationsQueryHandler(
    IOrganizationRepository organizationRepository,
    IUserRepository userRepository,
    ICurrentUserContext currentUserContext,
    IGlobalAdminChecker globalAdminChecker) : IRequestHandler<SearchOrganizationsQueryRequest, SearchOrganizationsQueryResult>
{
    public async Task<SearchOrganizationsQueryResult> Handle(SearchOrganizationsQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserContext.GetCurrentUserId();
        if (!currentUserId.HasValue)
            return new SearchOrganizationsQueryResult(Array.Empty<OrganizationDto>());

        var isGlobalAdmin = await globalAdminChecker.IsGlobalAdminAsync(currentUserId.Value, cancellationToken);

        IEnumerable<Domain.Entities.Organization> organizations;
        if (isGlobalAdmin)
        {
            organizations = await organizationRepository.GetAllAsync(cancellationToken);
        }
        else
        {
            var currentUser = await userRepository.GetByIdWithDetailsAsync(currentUserId.Value, cancellationToken);
            if (currentUser?.Organization == null)
                return new SearchOrganizationsQueryResult(Array.Empty<OrganizationDto>());

            var descendants = await organizationRepository.GetDescendantsAsync(currentUser.OrganizationId, cancellationToken);
            var orgIds = new[] { currentUser.OrganizationId }.Concat(descendants.Select(o => o.Id)).ToHashSet();
            var allOrgs = await organizationRepository.GetAllAsync(cancellationToken);
            organizations = allOrgs.Where(o => orgIds.Contains(o.Id));
        }

        organizations = organizations.Where(o => o.Name != "System");

        var nameFilter = (request.NamePartial ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(nameFilter))
        {
            organizations = organizations
                .Where(o => o.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (request.ParentId.HasValue)
        {
            organizations = organizations
                .Where(o => o.ParentId == request.ParentId)
                .ToList();
        }

        var items = organizations
            .Select(o => new OrganizationDto(o.Id, o.Name, o.ParentId))
            .ToList();

        return new SearchOrganizationsQueryResult(items);
    }
}

public record SearchOrganizationsQueryRequest(string? NamePartial, Guid? ParentId = null)
    : IRequest<SearchOrganizationsQueryResult>;

public record SearchOrganizationsQueryResult(IReadOnlyList<OrganizationDto> Organizations);

