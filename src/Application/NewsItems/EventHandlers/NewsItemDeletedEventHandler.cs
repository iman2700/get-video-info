using System.Reflection;
using Domain.Events;
using Application.Common.Interfaces;

namespace Application.NewsItems.EventHandlers;
public class NewsItemDeletedEventHandler : INotificationHandler<NewsItemDeletedEvent>
{
    private readonly ILogger _logger;

    public NewsItemDeletedEventHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(NewsItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information($"Domain Event: {notification.GetType().Name}", MethodBase.GetCurrentMethod());

        return Task.CompletedTask;
    }
}
