using Domain.Entities;
using Domain.Enums;
using Infrastructure.Services.Crawlers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.HostedServices;

public class CrawlerHostedService : IHostedService, IDisposable
{
    private Timer? _timer = null;
    
    private AuxaliaVideoCrawler _auxaliaVideoCrawler;
    private IConfiguration _configuration;
    public IServiceProvider _services;

    public CrawlerHostedService(IConfiguration configuration,
        IServiceProvider services,
        AuxaliaVideoCrawler auxaliaVideoCrawler)
    {
        _services = services;
        _auxaliaVideoCrawler = auxaliaVideoCrawler;
        _configuration = configuration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        int? fetchInterval = _configuration.GetValue<int>("Crawler:FetchInterval");
        Guard.Against.Null(fetchInterval);
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds((int)fetchInterval));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        List<NewsItem> auxaliaVideoList = await _auxaliaVideoCrawler.FetchNews();
        using var scope = _services.CreateScope();
        var applicationDbContext =
            scope.ServiceProvider
                .GetRequiredService<IApplicationDbContext>();
        var savedUrls = applicationDbContext.NewsItems.Select(n => n.Url).ToList();
        foreach (var auxaliaVideo in auxaliaVideoList.Where(auxaliaVideo => !savedUrls.Contains(auxaliaVideo.Url)))
            applicationDbContext.NewsItems.Add(auxaliaVideo);
        await applicationDbContext.SaveChangesAsync(CancellationToken.None);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
