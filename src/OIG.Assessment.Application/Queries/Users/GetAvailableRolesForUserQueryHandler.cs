using AutoMapper;
using MediatR;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain.Repositories;
using OIG.Assessment.Domain.Services;

namespace OIG.Assessment.Application.Queries.Users;

public class GetAvailableRolesForUserQueryHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPermissionService permissionService,
    ICurrentUserContext currentUserContext,
    IMapper mapper) : IRequestHandler<GetAvailableRolesForUserQueryRequest, GetAvailableRolesForUserQueryResult?>
{
    public async Task<GetAvailableRolesForUserQueryResult?> Handle(GetAvailableRolesForUserQueryRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithDetailsAsync(request.UserId, cancellationToken);
        if (user == null)
            return null;

        var allRoles = await roleRepository.GetAllWithDetailsAsync(cancellationToken);
        var availableRoles = permissionService.GetAvailableRolesForUser(user, allRoles);

        var currentUserId = currentUserContext.GetCurrentUserId();
        if (currentUserId.HasValue)
        {
            var currentUser = await userRepository.GetByIdWithDetailsAsync(currentUserId.Value, cancellationToken);
            if (currentUser != null)
                availableRoles = await permissionService.GetAssignableRolesAsync(currentUser, availableRoles, cancellationToken);
        }

        var roleDtos = mapper.Map<List<RoleDtos>>(availableRoles);
        return new GetAvailableRolesForUserQueryResult(roleDtos);
    }
}

public record GetAvailableRolesForUserQueryRequest(Guid UserId) : IRequest<GetAvailableRolesForUserQueryResult?>;

public record GetAvailableRolesForUserQueryResult(IReadOnlyList<RoleDtos> Roles);

