namespace Domain.Events;
public class ArticleCreatedEvent : BaseEvent
{
    public ArticleCreatedEvent(Article item)
    {
        Item = item;
    }

    public Article Item { get; }
}