using System.Reflection;
using Application.Common.Interfaces;

namespace Application.Common.Behaviours;
public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger _logger;

    public UnhandledExceptionBehaviour(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.Error(ex, MethodBase.GetCurrentMethod(),
                $"Request: Unhandled Exception for Request {requestName} {request}");
            throw;
        }
    }
}
