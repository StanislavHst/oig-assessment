using MediatR;
using OIG.Assessment.Domain.Entities;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Commands.Roles;

public class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateRoleCommandRequest, CreateRoleCommandResult>
{
    public async Task<CreateRoleCommandResult> Handle(CreateRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
            throw new InvalidOperationException($"Organization with id '{request.OrganizationId}' not found.");

        var role = new Role
        {
            Name = request.Name,
            OrganizationId = request.OrganizationId,
            Permissions = request.Permissions?.ToList() ?? new List<string>()
        };
        if (!role.ValidatePermissions())
            throw new InvalidOperationException("One or more permissions are invalid.");

        await roleRepository.AddAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateRoleCommandResult(role.Id);
    }
}

public record CreateRoleCommandRequest(string Name, Guid OrganizationId, IReadOnlyList<string>? Permissions = null)
    : IRequest<CreateRoleCommandResult>;

public record CreateRoleCommandResult(Guid RoleId);
