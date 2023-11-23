using Domain.Entities;

namespace Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<NewsItem> NewsItems { get; }

    DbSet<CategoryItem> CategoryItems { get; }

    DbSet<Tag> Tags { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
