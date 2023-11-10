namespace Domain.Entities;
public class CategoryItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsChecked { get; set; }
    public override string ToString()
    {
        return Name;
    }
}
