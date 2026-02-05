using MediatR;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain.Repositories;
using OIG.Assessment.Domain.Services;

namespace OIG.Assessment.Application.Commands.Users;

public class AssignRoleToUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPermissionService permissionService,
    ICurrentUserContext currentUserContext,
    IUnitOfWork unitOfWork) : IRequestHandler<AssignRoleToUserCommandRequest, Unit>
{
    public async Task<Unit> Handle(AssignRoleToUserCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithDetailsAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with id '{request.UserId}' not found.");

        var role = await roleRepository.GetByIdWithDetailsAsync(request.RoleId, cancellationToken);
        if (role == null)
            throw new InvalidOperationException($"Role with id '{request.RoleId}' not found.");

        var allRoles = await roleRepository.GetAllWithDetailsAsync(cancellationToken);
        var availableRoles = permissionService.GetAvailableRolesForUser(user, allRoles);

        if (!availableRoles.Any(r => r.Id == request.RoleId))
            throw new InvalidOperationException($"Role '{role.Name}' is not available for user '{user.Email}'. Role must belong to user's organization or any parent organization.");

        // Validate that the current user can assign this role
        var currentUserId = currentUserContext.GetCurrentUserId();
        if (currentUserId.HasValue)
        {
            var currentUser = await userRepository.GetByIdWithDetailsAsync(currentUserId.Value, cancellationToken);
            if (currentUser != null)
            {
                var assignableRoles = await permissionService.GetAssignableRolesAsync(currentUser, availableRoles, cancellationToken);
                if (!assignableRoles.Any(r => r.Id == request.RoleId))
                {
                    throw new InvalidOperationException($"You cannot assign role '{role.Name}' to user '{user.Email}'. You can only assign roles from your organization '{currentUser.Organization?.Name ?? "Unknown"}' and its child organizations.");
                }
            }
        }

        if (user.Roles.Any(r => r.Id == request.RoleId))
            throw new InvalidOperationException($"User '{user.Email}' already has role '{role.Name}'.");

        user.Roles.Add(role);
        user.UpdatedAt = DateTime.UtcNow;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public record AssignRoleToUserCommandRequest(
    Guid UserId,
    Guid RoleId
) : IRequest<Unit>;
