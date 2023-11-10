using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.NewsItems.EventHandlers;
public class NewsItemDeletedEventHandler : INotificationHandler<NewsItemDeletedEvent>
{
    private readonly ILogger<NewsItemDeletedEventHandler> _logger;

    public NewsItemDeletedEventHandler(ILogger<NewsItemDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(NewsItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
