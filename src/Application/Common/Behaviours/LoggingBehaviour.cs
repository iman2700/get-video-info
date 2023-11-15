using Application.Common.Interfaces;
using System.Reflection;

namespace Application.Common.Behaviours;
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger _logger;
    private readonly ICurrentUserService _user;
    private readonly IIdentityService _identityService;

    public LoggingBehavior(ILogger logger, ICurrentUserService user, IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.UserId ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        _logger.Information($"Request: {requestName} {userId} {userName} {request}", MethodBase.GetCurrentMethod());
        return await next();
    }
}