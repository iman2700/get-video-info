using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;

namespace Application.NewsItems.Commands.CreateNewsItem;
public record CreateNewsItemCommand : IRequest<int>
{
    public required string Title { get; set; }
    public string? NewsContent { get; set; }
    public string? Thumbnail { get; set; }
    public List<int>? CategoryItemIds { get; set; }
    public required Platform Source { get; set; }
    public required string Url { get; set; }
    public bool IsPublished { get; set; }
    public List<int>? TagIds { get; set; }
}

public class CreateNewsItemCommandHandler : IRequestHandler<CreateNewsItemCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateNewsItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateNewsItemCommand request, CancellationToken cancellationToken)
    {
        List<CategoryItem> categoryItems = new List<CategoryItem>();
        List<Tag> tags = new List<Tag>();
        if (request.CategoryItemIds != null)
            categoryItems.AddRange(_context.CategoryItems.Where(c => request.CategoryItemIds.Contains(c.Id)));
        if (request.TagIds != null)
            tags.AddRange(_context.Tags.Where(t => request.TagIds.Contains(t.Id)));

        var entity = new NewsItem
        {
            Title = request.Title,
            NewsContent = request.NewsContent,
            Thumbnail = request.Thumbnail,
            CategoryItems = categoryItems,
            Source = request.Source,
            Url = request.Url,
            IsPublished = request.IsPublished,
            Tags = tags
        };

        entity.AddDomainEvent(new NewsItemCreatedEvent(entity));

        _context.NewsItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
