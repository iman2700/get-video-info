namespace Domain.Entities;

public class NewsItem : BaseAuditableEntity
{
    public required string Title { get; set; }
    public string? NewsContent { get; set; }
    public string? Thumbnail { get; set; }
    public List<CategoryItem>? CategoryItems { get; set; }
    public Platform Source { get; set; }
    public string? Url { get; set; }

    public override string ToString()
    {
        return Title;
    }
}
