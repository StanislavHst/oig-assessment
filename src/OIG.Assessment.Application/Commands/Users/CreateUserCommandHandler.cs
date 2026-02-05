using MediatR;
using OIG.Assessment.Domain.Entities;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Commands.Users;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResult>
{
    public async Task<CreateUserCommandResult> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
            throw new InvalidOperationException($"User with email '{request.Email}' already exists.");

        var organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
            throw new InvalidOperationException($"Organization with id '{request.OrganizationId}' not found.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            OrganizationId = request.OrganizationId
        };

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateUserCommandResult(user.Id);
    }
}

public record CreateUserCommandRequest(
    string Name,
    string Email,
    Guid OrganizationId
) : IRequest<CreateUserCommandResult>;

public record CreateUserCommandResult(Guid UserId);
