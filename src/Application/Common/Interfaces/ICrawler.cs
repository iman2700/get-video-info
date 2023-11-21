using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Interfaces;
public interface ICrawler
{
    List<NewsItem> FetchNews(Platform platform, DateTime fromDateTime);
}
