using Application.Common.Mappings;
using Domain.Entities;
using Domain.Enums;

namespace Application.NewsItems.Queries.GetNewsItemsWithPagination;

public class NewsItemPageDto : IMapFrom<NewsItem>
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? NewsContent { get; set; }
    public string? Thumbnail { get; set; }
    public List<CategoryItemBriefDto>? CategoryItems { get; set; }
    public Platform Source { get; set; }
    public string? Url { get; set; }
}

public class CategoryItemBriefDto : IMapFrom<CategoryItem>
{
    public int Id { get; set; }
    public string Name { get; set; }
}