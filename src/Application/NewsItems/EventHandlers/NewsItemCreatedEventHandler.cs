using System.Reflection;
using Domain.Events;
using Application.Common.Interfaces;

namespace Application.NewsItems.EventHandlers;
internal class NewsItemCreatedEventHandler : INotificationHandler<NewsItemCreatedEvent>
{
    private readonly ILogger _logger;

    public NewsItemCreatedEventHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(NewsItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information($"Domain Event: {notification.GetType().Name}", MethodBase.GetCurrentMethod());

        return Task.CompletedTask;
    }
}
