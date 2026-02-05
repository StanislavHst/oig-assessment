using FluentValidation;

namespace OIG.Assessment.Application.Commands.Users.Validators;

public class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommandRequest>
{
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");
    }
}
