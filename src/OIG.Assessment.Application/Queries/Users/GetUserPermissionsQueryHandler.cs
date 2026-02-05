using MediatR;
using OIG.Assessment.Domain.Repositories;
using OIG.Assessment.Domain.Services;

namespace OIG.Assessment.Application.Queries.Users;

public class GetUserPermissionsQueryHandler(
    IUserRepository userRepository,
    IPermissionService permissionService) : IRequestHandler<GetUserPermissionsQueryRequest, GetUserPermissionsQueryResult?>
{
    public async Task<GetUserPermissionsQueryResult?> Handle(GetUserPermissionsQueryRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithDetailsAsync(request.UserId, cancellationToken);
        if (user == null)
            return null;
        var permissions = permissionService.GetUserPermissions(user);
        return new GetUserPermissionsQueryResult(permissions.ToList());
    }
}

public record GetUserPermissionsQueryRequest(Guid UserId) : IRequest<GetUserPermissionsQueryResult?>;

public record GetUserPermissionsQueryResult(IReadOnlyList<string> Permissions);
