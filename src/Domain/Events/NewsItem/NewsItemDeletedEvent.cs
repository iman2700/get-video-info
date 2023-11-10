namespace Domain.Events;
public class NewsItemDeletedEvent : BaseEvent
{
    public NewsItemDeletedEvent(NewsItem item)
    {
        Item = item;
    }

    public NewsItem Item { get; }
}