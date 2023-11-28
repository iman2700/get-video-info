using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.NewsItems.Commands.UpdateNewsItem;

public record UpdateNewsItemCommand : IRequest
{
    public int Id { get; init; }
    public required string Title { get; set; }
    public string? NewsContent { get; set; }
    public string? Thumbnail { get; set; }
    public List<int>? CategoryItemIds { get; set; }
    public required Platform Source { get; set; }
    public required string Url { get; set; }
    public bool IsPublished { get; set; }
    public List<int>? TagIds { get; set; }
}

public class UpdateNewsItemCommandHandler : IRequestHandler<UpdateNewsItemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateNewsItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateNewsItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.NewsItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        List<CategoryItem> categoryItems = new List<CategoryItem>();
        List<Tag> tags = new List<Tag>();
        if (request.CategoryItemIds != null)
            categoryItems.AddRange(_context.CategoryItems.Where(c => request.CategoryItemIds.Contains(c.Id)));
        if (request.TagIds != null)
            tags.AddRange(_context.Tags.Where(t => request.TagIds.Contains(t.Id)));

        entity.Title = request.Title;
        entity.NewsContent = request.NewsContent;
        entity.Thumbnail = request.Thumbnail;
        if (categoryItems.Count > 0)
            entity.CategoryItems = categoryItems;
        else
            entity.CategoryItems = null;
        entity.Source = request.Source;
        entity.Url = request.Url;
        entity.IsPublished = request.IsPublished;
        if (tags.Count > 0)
            entity.Tags = tags;
        else
            entity.Tags = null;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
