namespace Domain.Events;
public class CategoryItemDeletedEvent : BaseEvent
{
    public CategoryItemDeletedEvent(CategoryItem item)
    {
        Item = item;
    }

    public CategoryItem Item { get; }
}