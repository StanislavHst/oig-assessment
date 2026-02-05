using System.Threading;
using System.Threading.Tasks;

namespace OIG.Assessment.Application.Permissions;

public interface IPermissionChecker
{
    Task<IReadOnlyList<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);
}

