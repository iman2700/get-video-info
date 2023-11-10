using Application.Common.Interfaces;

namespace Application.Users.Commands.DeleteUser;
public record DeleteUserCommand(string Username) : IRequest<bool>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand,bool>
{
    private readonly IIdentityService _identityService;

    public DeleteUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.DeleteUserByUsernameAsync(request.Username);
        return result.Succeeded;
    }

}
