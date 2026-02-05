using MediatR;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Queries.Organizations;

public class GetOrganizationHierarchyQueryHandler(
    IOrganizationRepository organizationRepository,
    IUserRepository userRepository,
    ICurrentUserContext currentUserContext,
    IGlobalAdminChecker globalAdminChecker) : IRequestHandler<GetOrganizationHierarchyQueryRequest, GetOrganizationHierarchyQueryResult>
{
    public async Task<GetOrganizationHierarchyQueryResult> Handle(GetOrganizationHierarchyQueryRequest request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdWithChildrenAsync(request.OrganizationId, cancellationToken);
        if (organization == null || organization.Name == "System")
            throw new InvalidOperationException($"Organization with id '{request.OrganizationId}' not found.");

        var currentUserId = currentUserContext.GetCurrentUserId();
        if (currentUserId.HasValue && !await globalAdminChecker.IsGlobalAdminAsync(currentUserId.Value, cancellationToken))
        {
            var currentUser = await userRepository.GetByIdWithDetailsAsync(currentUserId.Value, cancellationToken);
            if (currentUser?.Organization == null)
                throw new InvalidOperationException($"Organization with id '{request.OrganizationId}' not found.");
            var descendants = await organizationRepository.GetDescendantsAsync(currentUser.OrganizationId, cancellationToken);
            var ancestors = await organizationRepository.GetAncestorsAsync(organization.Id, cancellationToken);
            var hasAccess = currentUser.OrganizationId == organization.Id ||
                           descendants.Any(d => d.Id == organization.Id) ||
                           ancestors.Any(a => a.Id == currentUser.OrganizationId);
            if (!hasAccess)
                throw new InvalidOperationException($"Organization with id '{request.OrganizationId}' not found.");
        }

        var root = MapToHierarchyItem(organization);
        return new GetOrganizationHierarchyQueryResult(root);
    }

    private static OrganizationHierarchyItemDto MapToHierarchyItem(Domain.Entities.Organization organization)
    {
        var children = organization.Children
            .Select(MapToHierarchyItem)
            .ToList();

        return new OrganizationHierarchyItemDto(
            organization.Id,
            organization.Name,
            organization.ParentId,
            children);
    }
}

public record GetOrganizationHierarchyQueryRequest(Guid OrganizationId) : IRequest<GetOrganizationHierarchyQueryResult>;

public record GetOrganizationHierarchyQueryResult(OrganizationHierarchyItemDto Organization);

