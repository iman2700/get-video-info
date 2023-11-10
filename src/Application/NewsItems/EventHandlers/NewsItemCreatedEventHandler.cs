using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.NewsItems.EventHandlers;
internal class NewsItemCreatedEventHandler : INotificationHandler<NewsItemCreatedEvent>
{
    private readonly ILogger<NewsItemCreatedEventHandler> _logger;

    public NewsItemCreatedEventHandler(ILogger<NewsItemCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(NewsItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
