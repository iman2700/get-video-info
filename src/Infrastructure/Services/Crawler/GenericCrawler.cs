using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services.Crawler;

internal class GenericCrawler : ICrawler
{
    private AuxaliaBlogCrawler _auxaliaBlogCrawler;
    public GenericCrawler(AuxaliaBlogCrawler auxaliaBlogCrawler)
    {
        _auxaliaBlogCrawler = auxaliaBlogCrawler;
    }
    public List<NewsItem> FetchNews(Platform platform, DateTime fromDateTime)
    {
        List<NewsItem> returnValue = new List<NewsItem>();
        switch (platform)
        {
            case Platform.AuxaliaBlog:
                returnValue.AddRange(_auxaliaBlogCrawler.FetchNews(fromDateTime));
                break;
        }
        return returnValue;
    }
}
