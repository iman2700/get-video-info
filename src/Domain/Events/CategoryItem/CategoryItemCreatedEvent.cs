namespace Domain.Events;
public class CategoryItemCreatedEvent : BaseEvent
{
    public CategoryItemCreatedEvent(CategoryItem item)
    {
        Item = item;
    }

    public CategoryItem Item { get; }
}