using MediatR;
using OIG.Assessment.Domain.Entities;
using OIG.Assessment.Domain.Repositories;

namespace OIG.Assessment.Application.Commands.Organizations;

public class CreateOrganizationCommandHandler(
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOrganizationCommandRequest, CreateOrganizationCommandResult>
{
    public async Task<CreateOrganizationCommandResult> Handle(CreateOrganizationCommandRequest request, CancellationToken cancellationToken)
    {
        Guid? effectiveParentId = request.ParentId;
        if (!effectiveParentId.HasValue)
        {
            var systemOrg = await organizationRepository.GetSystemOrganizationAsync(cancellationToken);
            if (systemOrg == null)
                throw new InvalidOperationException("System organization not found. Root organizations must be created under System.");
            effectiveParentId = systemOrg.Id;
        }
        else
        {
            var parent = await organizationRepository.GetByIdAsync(effectiveParentId.Value, cancellationToken);
            if (parent == null)
                throw new InvalidOperationException($"Parent organization with id '{effectiveParentId}' not found.");
        }

        var organization = new Organization
        {
            Name = request.Name,
            ParentId = effectiveParentId
        };

        await organizationRepository.AddAsync(organization, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOrganizationCommandResult(organization.Id);
    }
}

public record CreateOrganizationCommandRequest(string Name, Guid? ParentId) : IRequest<CreateOrganizationCommandResult>;

public record CreateOrganizationCommandResult(Guid OrganizationId);

