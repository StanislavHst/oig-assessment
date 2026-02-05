using FluentValidation;

namespace OIG.Assessment.Application.Commands.Users.Validators;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommandRequest>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}
