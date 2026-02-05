using MediatR;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Commands.Roles;

public class DeleteRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteRoleCommandRequest, Unit>
{
    public async Task<Unit> Handle(DeleteRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
            throw new InvalidOperationException($"Role with id '{request.RoleId}' not found.");

        roleRepository.Remove(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public record DeleteRoleCommandRequest(Guid RoleId) : IRequest<Unit>;
