using MediatR;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Queries.Organizations;

public class GetOrganizationByIdQueryHandler(
    IOrganizationRepository organizationRepository,
    IUserRepository userRepository,
    ICurrentUserContext currentUserContext,
    IGlobalAdminChecker globalAdminChecker) : IRequestHandler<GetOrganizationByIdQueryRequest, GetOrganizationByIdQueryResult?>
{
    public async Task<GetOrganizationByIdQueryResult?> Handle(GetOrganizationByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null || organization.Name == "System")
            return null;

        var currentUserId = currentUserContext.GetCurrentUserId();
        if (currentUserId.HasValue && !await globalAdminChecker.IsGlobalAdminAsync(currentUserId.Value, cancellationToken))
        {
            var currentUser = await userRepository.GetByIdWithDetailsAsync(currentUserId.Value, cancellationToken);
            if (currentUser?.Organization == null)
                return null;
            var descendants = await organizationRepository.GetDescendantsAsync(currentUser.OrganizationId, cancellationToken);
            var ancestors = await organizationRepository.GetAncestorsAsync(organization.Id, cancellationToken);
            var hasAccess = currentUser.OrganizationId == organization.Id ||
                           descendants.Any(d => d.Id == organization.Id) ||
                           ancestors.Any(a => a.Id == currentUser.OrganizationId);
            if (!hasAccess)
                return null;
        }

        var dto = new OrganizationDto(organization.Id, organization.Name, organization.ParentId);
        return new GetOrganizationByIdQueryResult(dto);
    }
}

public record GetOrganizationByIdQueryRequest(Guid OrganizationId) : IRequest<GetOrganizationByIdQueryResult?>;

public record GetOrganizationByIdQueryResult(OrganizationDto Organization);

