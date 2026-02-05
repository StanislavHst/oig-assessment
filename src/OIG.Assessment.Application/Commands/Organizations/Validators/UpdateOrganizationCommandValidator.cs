using FluentValidation;

namespace OIG.Assessment.Application.Commands.Organizations.Validators;

public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommandRequest>
{
    public UpdateOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("OrganizationId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}

