using AutoMapper;
using MediatR;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Queries.Roles;

public class SearchRolesQueryHandler(
    IRoleRepository roleRepository,
    IUserRepository userRepository,
    IOrganizationRepository organizationRepository,
    ICurrentUserContext currentUserContext,
    IGlobalAdminChecker globalAdminChecker,
    IMapper mapper) : IRequestHandler<SearchRolesQueryRequest, SearchRolesQueryResult>
{
    public async Task<SearchRolesQueryResult> Handle(SearchRolesQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserContext.GetCurrentUserId();
        if (!currentUserId.HasValue)
            return new SearchRolesQueryResult(Array.Empty<RoleListItemDto>());

        var isGlobalAdmin = await globalAdminChecker.IsGlobalAdminAsync(currentUserId.Value, cancellationToken);
        Guid? effectiveOrgId = request.OrganizationId;

        if (!isGlobalAdmin)
        {
            var currentUser = await userRepository.GetByIdWithDetailsAsync(currentUserId.Value, cancellationToken);
            if (currentUser?.Organization == null)
                return new SearchRolesQueryResult(Array.Empty<RoleListItemDto>());

            if (!effectiveOrgId.HasValue || effectiveOrgId.Value == Guid.Empty)
            {
                effectiveOrgId = currentUser.OrganizationId;
            }
            else
            {
                var descendants = await organizationRepository.GetDescendantsAsync(currentUser.OrganizationId, cancellationToken);
                var isInBranch = currentUser.OrganizationId == effectiveOrgId.Value ||
                                 descendants.Any(d => d.Id == effectiveOrgId.Value);
                if (!isInBranch)
                    return new SearchRolesQueryResult(Array.Empty<RoleListItemDto>());
            }
        }

        IEnumerable<Domain.Entities.Role> roles;
        if (effectiveOrgId == null || effectiveOrgId == Guid.Empty)
        {
            roles = await roleRepository.GetAllWithDetailsAsync(cancellationToken);
        }
        else
        {
            var orgIds = new[] { effectiveOrgId.Value }
                .Concat((await organizationRepository.GetDescendantsAsync(effectiveOrgId.Value, cancellationToken)).Select(o => o.Id));
            roles = await roleRepository.GetByOrganizationIdsAsync(orgIds, cancellationToken);
        }

        var nameFilter = (request.NamePartial ?? "").Trim();
        if (!string.IsNullOrEmpty(nameFilter))
            roles = roles.Where(r => r.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        var items = mapper.Map<List<RoleListItemDto>>(roles);
        return new SearchRolesQueryResult(items);
    }
}

public record SearchRolesQueryRequest(string? NamePartial, Guid? OrganizationId = null) : IRequest<SearchRolesQueryResult>;

public record SearchRolesQueryResult(IReadOnlyList<RoleListItemDto> Roles);
