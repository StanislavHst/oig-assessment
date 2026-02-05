using MediatR;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Commands.Organizations;

public class DeleteOrganizationCommandHandler(
    IOrganizationRepository organizationRepository,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ICurrentUserContext currentUserContext,
    IGlobalAdminChecker globalAdminChecker,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteOrganizationCommandRequest, Unit>
{
    public async Task<Unit> Handle(DeleteOrganizationCommandRequest request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdWithChildrenAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
            throw new InvalidOperationException($"Organization with id '{request.OrganizationId}' not found.");

        if (organization.Name == "System")
            throw new InvalidOperationException("System organization cannot be deleted.");

        var currentUserId = currentUserContext.GetCurrentUserId();
        if (currentUserId.HasValue && !await globalAdminChecker.IsGlobalAdminAsync(currentUserId.Value, cancellationToken))
        {
            var currentUser = await userRepository.GetByIdWithDetailsAsync(currentUserId.Value, cancellationToken);
            if (currentUser?.Organization == null)
                throw new InvalidOperationException("Access denied.");
            var descendants = await organizationRepository.GetDescendantsAsync(currentUser.OrganizationId, cancellationToken);
            var ancestors = await organizationRepository.GetAncestorsAsync(organization.Id, cancellationToken);
            var hasAccess = currentUser.OrganizationId == organization.Id ||
                           descendants.Any(d => d.Id == organization.Id) ||
                           ancestors.Any(a => a.Id == currentUser.OrganizationId);
            if (!hasAccess)
                throw new InvalidOperationException("Access denied.");
        }

        await DeleteOrganizationRecursiveAsync(organization, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task DeleteOrganizationRecursiveAsync(Domain.Entities.Organization organization, CancellationToken cancellationToken)
    {
        var children = organization.Children.ToList();
        foreach (var child in children)
        {
            var childWithDetails = await organizationRepository.GetByIdWithChildrenAsync(child.Id, cancellationToken);
            if (childWithDetails != null)
                await DeleteOrganizationRecursiveAsync(childWithDetails, cancellationToken);
        }

        foreach (var user in organization.Users.ToList())
        {
            userRepository.Remove(user);
        }

        foreach (var role in organization.Roles.ToList())
        {
            roleRepository.Remove(role);
        }

        organizationRepository.Remove(organization);
    }
}

public record DeleteOrganizationCommandRequest(Guid OrganizationId) : IRequest<Unit>;

