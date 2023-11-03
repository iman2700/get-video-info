using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Articles.EventHandlers;
public class ArticleDeletedEventHandler : INotificationHandler<ArticleDeletedEvent>
{
    private readonly ILogger<ArticleDeletedEventHandler> _logger;

    public ArticleDeletedEventHandler(ILogger<ArticleDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ArticleDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
