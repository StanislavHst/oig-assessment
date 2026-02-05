using AutoMapper;
using MediatR;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Queries.Users;

public class GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    : IRequestHandler<GetUserByIdQueryRequest, GetUserByIdQueryResult?>
{
    public async Task<GetUserByIdQueryResult?> Handle(GetUserByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithDetailsAsync(request.UserId, cancellationToken);
        if (user == null)
            return null;
        return mapper.Map<GetUserByIdQueryResult>(user);
    }
}

public record GetUserByIdQueryRequest(Guid UserId) : IRequest<GetUserByIdQueryResult?>;

public record GetUserByIdQueryResult(
    Guid Id,
    string Name,
    string Email,
    Guid OrganizationId,
    string OrganizationName,
    IReadOnlyList<RoleDtos> Roles,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
