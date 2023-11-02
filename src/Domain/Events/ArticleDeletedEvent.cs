using Domain.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events;
public class ArticleDeletedEvent : BaseEvent
{
    public ArticleDeletedEvent(Article item)
    {
        Item = item;
    }

    public Article Item { get; }
}