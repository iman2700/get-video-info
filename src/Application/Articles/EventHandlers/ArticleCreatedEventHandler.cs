using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Events;

namespace Application.Articles.EventHandlers;
internal class ArticleCreatedEventHandler : INotificationHandler<ArticleCreatedEvent>
{
    private readonly ILogger<ArticleCreatedEventHandler> _logger;

    public ArticleCreatedEventHandler(ILogger<ArticleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ArticleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
