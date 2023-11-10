using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.CategoryItems.EventHandlers;
public class CategoryItemCreatedEventHandler : INotificationHandler<CategoryItemCreatedEvent>
{
    private readonly ILogger<CategoryItemCreatedEventHandler> _logger;

    public CategoryItemCreatedEventHandler(ILogger<CategoryItemCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CategoryItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
