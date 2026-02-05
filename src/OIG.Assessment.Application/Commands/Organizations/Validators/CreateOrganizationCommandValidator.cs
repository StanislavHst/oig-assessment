using FluentValidation;

namespace OIG.Assessment.Application.Commands.Organizations.Validators;

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommandRequest>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}

