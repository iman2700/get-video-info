using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Events;
public class ArticleCreatedEvent : BaseEvent
{
    public ArticleCreatedEvent(Article item)
    {
        Item = item;
    }

    public Article Item { get; }
}