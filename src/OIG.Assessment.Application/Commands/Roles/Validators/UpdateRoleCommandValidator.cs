using FluentValidation;

namespace OIG.Assessment.Application.Commands.Roles.Validators;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommandRequest>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}
