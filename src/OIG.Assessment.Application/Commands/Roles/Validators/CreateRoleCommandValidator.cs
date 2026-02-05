using FluentValidation;

namespace OIG.Assessment.Application.Commands.Roles.Validators;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommandRequest>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("OrganizationId is required.");
    }
}
