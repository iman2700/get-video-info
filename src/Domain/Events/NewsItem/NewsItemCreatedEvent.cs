namespace Domain.Events;
public class NewsItemCreatedEvent : BaseEvent
{
    public NewsItemCreatedEvent(NewsItem item)
    {
        Item = item;
    }

    public NewsItem Item { get; }
}