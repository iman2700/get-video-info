namespace Domain.Entities;

public class Article : BaseAuditableEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Thumbnail { get; set; }

}
