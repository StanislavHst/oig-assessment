using AutoMapper;
using MediatR;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Queries.Users;

public class SearchUsersQueryHandler(
    IUserRepository userRepository,
    IOrganizationRepository organizationRepository,
    ICurrentUserContext currentUserContext,
    IGlobalAdminChecker globalAdminChecker,
    IMapper mapper) : IRequestHandler<SearchUsersQueryRequest, SearchUsersQueryResult>
{
    public async Task<SearchUsersQueryResult> Handle(SearchUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserContext.GetCurrentUserId();
        if (!currentUserId.HasValue)
            return new SearchUsersQueryResult(Array.Empty<UserDto>());

        var isGlobalAdmin = await globalAdminChecker.IsGlobalAdminAsync(currentUserId.Value, cancellationToken);
        Guid? effectiveOrgId = request.OrganizationId;

        if (!isGlobalAdmin)
        {
            var currentUser = await userRepository.GetByIdWithDetailsAsync(currentUserId.Value, cancellationToken);
            if (currentUser?.Organization == null)
                return new SearchUsersQueryResult(Array.Empty<UserDto>());

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
                    return new SearchUsersQueryResult(Array.Empty<UserDto>());
            }
        }

        var users = effectiveOrgId == null || effectiveOrgId == Guid.Empty
            ? await userRepository.GetAllWithDetailsAsync(cancellationToken)
            : await userRepository.GetByOrganizationIdWithDescendantsAsync(effectiveOrgId.Value, cancellationToken);
        var nameFilter = (request.NamePartial ?? "").Trim();
        if (!string.IsNullOrEmpty(nameFilter))
            users = users.Where(u => u.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        var items = mapper.Map<List<UserDto>>(users);
        return new SearchUsersQueryResult(items);
    }
}

public record SearchUsersQueryRequest(string? NamePartial, Guid? OrganizationId = null) : IRequest<SearchUsersQueryResult>;

public record SearchUsersQueryResult(IReadOnlyList<UserDto> Users);
