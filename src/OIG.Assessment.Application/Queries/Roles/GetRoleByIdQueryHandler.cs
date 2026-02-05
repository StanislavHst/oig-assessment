using AutoMapper;
using MediatR;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Queries.Roles;

public class GetRoleByIdQueryHandler(IRoleRepository roleRepository, IMapper mapper)
    : IRequestHandler<GetRoleByIdQueryRequest, GetRoleByIdQueryResult?>
{
    public async Task<GetRoleByIdQueryResult?> Handle(GetRoleByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdWithDetailsAsync(request.RoleId, cancellationToken);
        if (role == null)
            return null;
        return mapper.Map<GetRoleByIdQueryResult>(role);
    }
}

public record GetRoleByIdQueryRequest(Guid RoleId) : IRequest<GetRoleByIdQueryResult?>;

public record GetRoleByIdQueryResult(
    Guid Id,
    string Name,
    Guid OrganizationId,
    string OrganizationName,
    IReadOnlyList<string> Permissions,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
