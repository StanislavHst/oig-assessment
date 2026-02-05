using AutoMapper;
using MediatR;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Queries.Users;

public class GetUsersForSelectorQueryHandler(IUserRepository userRepository, IMapper mapper)
    : IRequestHandler<GetUsersForSelectorQueryRequest, GetUsersForSelectorQueryResult>
{
    public async Task<GetUsersForSelectorQueryResult> Handle(GetUsersForSelectorQueryRequest request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllWithDetailsAsync(cancellationToken);
        var items = mapper.Map<List<UserDto>>(users);
        return new GetUsersForSelectorQueryResult(items);
    }
}

public record GetUsersForSelectorQueryRequest : IRequest<GetUsersForSelectorQueryResult>;

public record GetUsersForSelectorQueryResult(IReadOnlyList<UserDto> Users);
