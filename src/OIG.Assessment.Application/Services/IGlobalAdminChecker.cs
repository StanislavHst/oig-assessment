using System.Threading;
using System.Threading.Tasks;

namespace OIG.Assessment.Application.Services;

public interface IGlobalAdminChecker
{
    Task<bool> IsGlobalAdminAsync(Guid userId, CancellationToken cancellationToken = default);
}
