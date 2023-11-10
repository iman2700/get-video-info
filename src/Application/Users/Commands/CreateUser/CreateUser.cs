using Application.Common.Interfaces;

namespace Application.Users.Commands.CreateUser;
public record CreateUserCommand : IRequest<string>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.CreateUserAsync(request.Username, request.Password, request.Email);
        if (result.Result.Succeeded)
            return result.UserId;
        return "";
    }
}
