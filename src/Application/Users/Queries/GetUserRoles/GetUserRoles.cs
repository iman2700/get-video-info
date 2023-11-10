using Application.Common.Interfaces;

namespace Application.Users.Queries.GetUserRoles;

public record GetUserRolesQuery(string UserId) : IRequest<IList<string>>;
public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IList<string>>
{
    private readonly IIdentityService _identityService;
    public GetUserRolesQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IList<string>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetRolesAsync(request.UserId);
    }
}