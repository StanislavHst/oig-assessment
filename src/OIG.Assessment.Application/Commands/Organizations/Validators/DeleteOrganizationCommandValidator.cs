using FluentValidation;

namespace OIG.Assessment.Application.Commands.Organizations.Validators;

public class DeleteOrganizationCommandValidator : AbstractValidator<DeleteOrganizationCommandRequest>
{
    public DeleteOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("OrganizationId is required.");
    }
}

