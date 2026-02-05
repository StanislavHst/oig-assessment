using FluentValidation;

namespace OIG.Assessment.Application.Commands.Roles.Validators;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommandRequest>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");
    }
}
