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
    public List<CategoryItem>? CategoryItems { get; set; }
    public Platform Source { get; set; }
    public string? Url { get; set; }
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

        entity.Title = request.Title;
        entity.NewsContent = request.NewsContent;
        entity.Thumbnail = request.Thumbnail;
        entity.CategoryItems = request.CategoryItems;
        entity.Source = request.Source;
        entity.Url = request.Url;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
