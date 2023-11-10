using Application.Common.Interfaces;
using Domain.Events;

namespace Application.NewsItems.Commands.DeleteNewsItem;
public record DeleteNewsItemCommand(int Id) : IRequest;

public class DeleteNewsItemCommandHandler : IRequestHandler<DeleteNewsItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteNewsItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteNewsItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.NewsItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.NewsItems.Remove(entity);

        entity.AddDomainEvent(new NewsItemDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);
    }

}