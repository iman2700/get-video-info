using System.Reflection;
using Domain.Events;
using Application.Common.Interfaces;

namespace Application.CategoryItems.EventHandlers;
public class CategoryItemCreatedEventHandler : INotificationHandler<CategoryItemCreatedEvent>
{
    private readonly ILogger _logger;

    public CategoryItemCreatedEventHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(CategoryItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information($"Domain Event: {notification.GetType().Name}",MethodBase.GetCurrentMethod());

        return Task.CompletedTask;
    }
}
