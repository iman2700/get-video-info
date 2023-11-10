using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.CategoryItems.EventHandlers;
public class CategoryItemDeletedEventHandler : INotificationHandler<CategoryItemDeletedEvent>
{
    private readonly ILogger<CategoryItemDeletedEventHandler> _logger;

    public CategoryItemDeletedEventHandler(ILogger<CategoryItemDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CategoryItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
