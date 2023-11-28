namespace Domain.Entities;

public class NewsItem : BaseAuditableEntity
{
    public required string Title { get; set; }
    public string? NewsContent { get; set; }
    public string? Thumbnail { get; set; }
    public List<CategoryItem>? CategoryItems { get; set; }
    public required Platform Source { get; set; }
    public required string Url { get; set; }
    public bool IsPublished { get; set; }
    public List<Tag> Tags { get; set; }

    public override string ToString()
    {
        return Title;
    }
}
