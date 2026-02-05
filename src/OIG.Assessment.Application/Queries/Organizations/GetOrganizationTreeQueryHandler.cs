using MediatR;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Queries.Organizations;

public class GetOrganizationTreeQueryHandler(IOrganizationRepository organizationRepository)
    : IRequestHandler<GetOrganizationTreeQueryRequest, GetOrganizationTreeQueryResult>
{
    public async Task<GetOrganizationTreeQueryResult> Handle(GetOrganizationTreeQueryRequest request, CancellationToken cancellationToken)
    {
        var roots = await organizationRepository.GetVisibleRootOrganizationsAsync(cancellationToken);
        var items = roots.Select(MapToHierarchyItem).ToList();
        return new GetOrganizationTreeQueryResult(items);
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

public record GetOrganizationTreeQueryRequest() : IRequest<GetOrganizationTreeQueryResult>;

public record GetOrganizationTreeQueryResult(IReadOnlyList<OrganizationHierarchyItemDto> Organizations);

