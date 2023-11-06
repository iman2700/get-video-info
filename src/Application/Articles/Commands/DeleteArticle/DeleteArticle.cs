﻿using Application.Common.Interfaces;
using Domain.Events;

namespace Application.Articles.Commands.DeleteArticle;
public record DeleteArticleCommand(int Id) : IRequest;

public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteArticleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Articles
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.Articles.Remove(entity);

        entity.AddDomainEvent(new ArticleDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);
    }

}