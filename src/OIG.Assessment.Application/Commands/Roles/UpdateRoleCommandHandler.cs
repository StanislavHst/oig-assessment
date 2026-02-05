using MediatR;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Commands.Roles;

public class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateRoleCommandRequest, Unit>
{
    public async Task<Unit> Handle(UpdateRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
            throw new InvalidOperationException($"Role with id '{request.RoleId}' not found.");

        role.Name = request.Name;
        role.Permissions = request.Permissions?.ToList() ?? new List<string>();
        role.UpdatedAt = DateTime.UtcNow;
        if (!role.ValidatePermissions())
            throw new InvalidOperationException("One or more permissions are invalid.");

        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public record UpdateRoleCommandRequest(Guid RoleId, string Name, IReadOnlyList<string>? Permissions = null)
    : IRequest<Unit>;
