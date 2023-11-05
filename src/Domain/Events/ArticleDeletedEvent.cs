namespace Domain.Events;
public class ArticleDeletedEvent : BaseEvent
{
    public ArticleDeletedEvent(Article item)
    {
        Item = item;
    }

    public Article Item { get; }
}