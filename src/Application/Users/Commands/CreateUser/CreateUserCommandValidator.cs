namespace Application.Users.Commands.CreateUser;
internal class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.Username)
            .MaximumLength(50).WithMessage("Username length must not exceed 50.")
            .NotEmpty().WithMessage("Username cannot be empty.");
        RuleFor(p => p.Password).NotEmpty().WithMessage("Password cannot be empty.")
            .MinimumLength(8).WithMessage("Password length must be at least 8.")
            .MaximumLength(16).WithMessage("Password length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Password must contain at least one number.");
        RuleFor(e => e.Email).EmailAddress();
    }
}