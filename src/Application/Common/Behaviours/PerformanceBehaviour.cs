using Application.Common.Interfaces;
using Application.Common.Interfaces;
using System.Diagnostics;
using System.Reflection;

namespace Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger _logger;
    private readonly ICurrentUserService _user;
    private readonly IIdentityService _identityService;

    public PerformanceBehaviour(
        ILogger logger,
        ICurrentUserService user,
        IIdentityService identityService)
    {
        _timer = new Stopwatch();

        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.UserId ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            _logger.Warning(
                $"Long Running Request: {requestName} ({elapsedMilliseconds} milliseconds) {userId} {userName} {request}",
                MethodBase.GetCurrentMethod());
        }

        return response;
    }
}
