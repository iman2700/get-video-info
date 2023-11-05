using Domain.Entities;

namespace Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<Article> Articles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
