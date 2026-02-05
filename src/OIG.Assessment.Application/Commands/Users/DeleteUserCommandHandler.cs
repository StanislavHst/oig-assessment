using MediatR;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Commands.Users;

public class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommandRequest, Unit>
{
    public async Task<Unit> Handle(DeleteUserCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with id '{request.UserId}' not found.");

        userRepository.Remove(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public record DeleteUserCommandRequest(Guid UserId) : IRequest<Unit>;
