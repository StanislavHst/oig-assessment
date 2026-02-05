using MediatR;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Commands.Organizations;

public class UpdateOrganizationCommandHandler(
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrganizationCommandRequest, Unit>
{
    public async Task<Unit> Handle(UpdateOrganizationCommandRequest request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
            throw new InvalidOperationException($"Organization with id '{request.OrganizationId}' not found.");

        organization.Name = request.Name;
        organization.UpdatedAt = DateTime.UtcNow;

        organizationRepository.Update(organization);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public record UpdateOrganizationCommandRequest(Guid OrganizationId, string Name) : IRequest<Unit>;

