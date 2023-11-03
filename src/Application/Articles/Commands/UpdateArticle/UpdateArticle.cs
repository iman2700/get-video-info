using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Ardalis.GuardClauses;
using MediatR;

namespace Application.Articles.Commands.UpdateArticle;

public record UpdateArticleCommand : IRequest
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? Thumbnail { get; init; }
}

public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateArticleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Articles
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.Thumbnail = request.Thumbnail;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
