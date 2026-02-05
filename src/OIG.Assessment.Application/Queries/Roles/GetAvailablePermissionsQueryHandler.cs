using MediatR;
using OIG.Assessment.Domain.Common;

namespace OIG.Assessment.Application.Queries.Roles;

public class GetAvailablePermissionsQueryHandler()
    : IRequestHandler<GetAvailablePermissionsQueryRequest, GetAvailablePermissionsQueryResult>
{
    public async Task<GetAvailablePermissionsQueryResult> Handle(GetAvailablePermissionsQueryRequest request, CancellationToken cancellationToken)
    {
        var permissions = OIG.Assessment.Domain.Common.Permissions.GetAll();
        return new GetAvailablePermissionsQueryResult(permissions);
    }
}

public record GetAvailablePermissionsQueryRequest() : IRequest<GetAvailablePermissionsQueryResult>;

public record GetAvailablePermissionsQueryResult(IReadOnlyList<string> Permissions);
