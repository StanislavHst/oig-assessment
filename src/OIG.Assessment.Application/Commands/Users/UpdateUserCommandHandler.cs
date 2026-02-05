using MediatR;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Commands.Users;

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommandRequest, Unit>
{
    public async Task<Unit> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with id '{request.UserId}' not found.");

        if (user.Email != request.Email)
        {
            var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
                throw new InvalidOperationException($"User with email '{request.Email}' already exists.");
        }

        var organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
            throw new InvalidOperationException($"Organization with id '{request.OrganizationId}' not found.");

        user.Name = request.Name;
        user.Email = request.Email;
        user.OrganizationId = request.OrganizationId;
        user.UpdatedAt = DateTime.UtcNow;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public record UpdateUserCommandRequest(
    Guid UserId,
    string Name,
    string Email,
    Guid OrganizationId
) : IRequest<Unit>;
