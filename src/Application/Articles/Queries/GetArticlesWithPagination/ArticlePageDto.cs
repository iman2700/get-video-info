using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Articles.Queries.GetArticlesWithPagination;

public class ArticlePageDto : IMapFrom<Article>
{
    public int Id { get; init; }
    public string Title { get; init; } = String.Empty;
    public string? Description { get; init; }
    public string? Thumbnail { get; init; }
}
