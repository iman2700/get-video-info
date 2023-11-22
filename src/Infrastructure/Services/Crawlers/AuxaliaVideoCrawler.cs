using Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services.Crawlers;
public class AuxaliaVideoCrawler
{
    private IConfiguration _configuration;
    public AuxaliaVideoCrawler(IConfiguration configuration)
    {
        _configuration = configuration;
        Guard.Against.Null(_configuration);
    }

    public async Task<List<NewsItem>> FetchNews()
    {
        List<NewsItem> newsItems = new List<NewsItem>();
        string? auxaliaVideoBaseUrl = _configuration.GetValue<string>("Crawler:AuxaliaVideoBaseUrl");
        string? auxaliaVideoGetAllUrl = _configuration.GetValue<string>("Crawler:AuxaliaVideoGetAllUrl");
        Guard.Against.NullOrEmpty(auxaliaVideoBaseUrl);
        Guard.Against.NullOrEmpty(auxaliaVideoGetAllUrl);

        var client = new RestClient(auxaliaVideoBaseUrl);
        var request = new RestRequest(auxaliaVideoGetAllUrl, Method.Get);
        RestResponse response = await client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            JArray videos = JsonConvert.DeserializeObject<JArray>(response.Content);
            foreach (JToken video in videos)
            {
                NewsItem newsItem = new NewsItem()
                {
                    Title = video["title"]?.ToString() ?? "",
                    NewsContent = video["description"]?.ToString() ?? "",
                    Url = $"https://www.youtube.com/watch?v={video["id"]?.ToString() ?? ""}",
                    Source = Platform.AuxaliaVideo,
                    Created = DateTimeOffset.Parse(video["data"]["publishedAt"].ToString())
                };
                newsItems.Add(newsItem);
            }
        }

        return newsItems;
    }
}
