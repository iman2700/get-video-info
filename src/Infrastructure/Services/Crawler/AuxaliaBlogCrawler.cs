using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ardalis.GuardClauses;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Crawler;
internal class AuxaliaBlogCrawler
{
    private IConfiguration _configuration;
    public AuxaliaBlogCrawler(IConfiguration configuration)
    {
        _configuration = configuration;
        Guard.Against.Null(_configuration);
    }
    public List<NewsItem> FetchNews(DateTime fromDateTime)
    {
        List<NewsItem> newsItems = new List<NewsItem>();
        int maxPageCheck = 5000;
        int page = 1000;
        string? auxaliaBlogUrl = _configuration.GetValue<string>("Crawler:AuxaliaBlogUrl");
        Guard.Against.NullOrEmpty(auxaliaBlogUrl);
        while (page < maxPageCheck)
        {
            string url = page > 1 ? $"{auxaliaBlogUrl}/?paged={page}" : auxaliaBlogUrl;
            IEnumerable<SyndicationItem>? items = null;
            try
            {
                var reader = XmlReader.Create(url);
                var feed = SyndicationFeed.Load(reader);
                items = feed.Items;
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    break;
                throw;
            }
            if (items == null)
                break;
            foreach (SyndicationItem syndicationItem in items)
            {
                if (syndicationItem.PublishDate <= fromDateTime)
                {
                    page = maxPageCheck;
                    break;
                }
                List<CategoryItem> categories = new List<CategoryItem>();
                foreach (SyndicationCategory syndicationItemCategory in syndicationItem.Categories)
                {
                    categories.Add(new CategoryItem()
                    {
                        Name = syndicationItemCategory.Name,
                        Description = syndicationItemCategory.Label
                    });
                }

                NewsItem newsItem = new NewsItem
                {
                    Source = Platform.AuxaliaBlog,
                    Title = syndicationItem.Title.Text,
                    NewsContent = syndicationItem.Summary.Text,
                    CategoryItems = categories,
                    Url = syndicationItem.Id,
                    Thumbnail = "",
                    Created = syndicationItem.PublishDate,
                    LastModified = syndicationItem.LastUpdatedTime
                };
                newsItems.Add(newsItem);
            }
            page++;
        }

        return newsItems;
    }
}
