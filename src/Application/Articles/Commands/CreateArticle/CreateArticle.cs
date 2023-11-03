using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events;

namespace Application.Articles.Commands.CreateArticle;
public record CreateArticleCommand : IRequest<int>
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Thumbnail { get; set; }
}

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateArticleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var entity = new Article
        {
            Title = request.Title,
            Description = request.Description,
            Thumbnail = request.Thumbnail,
        };

        entity.AddDomainEvent(new ArticleCreatedEvent(entity));

        _context.Articles.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
