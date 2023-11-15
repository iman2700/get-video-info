using System.Reflection;
using Domain.Events;
using Application.Common.Interfaces;

namespace Application.CategoryItems.EventHandlers;
public class CategoryItemDeletedEventHandler : INotificationHandler<CategoryItemDeletedEvent>
{
    private readonly ILogger _logger;

    public CategoryItemDeletedEventHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(CategoryItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information($"Domain Event: {notification.GetType().Name}", MethodBase.GetCurrentMethod());

        return Task.CompletedTask;
    }
}
