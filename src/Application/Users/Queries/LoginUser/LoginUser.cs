using Application.Articles.Queries.GetArticlesWithPagination;
using Application.Common.Interfaces;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.LoginUser;

public record LoginUserQuery : IRequest<string>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, string>
{
    private readonly IIdentityService _identityService;
    public LoginUserQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request.Username, request.Password);
    }
}