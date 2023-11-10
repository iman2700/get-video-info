using Application.Common.Mappings;
using Domain.Entities;

namespace Application.CategoryItems.Queries.GetCategoryItemsWithPagination;

public class CategoryItemPageDto : IMapFrom<CategoryItem>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsChecked { get; set; }
}