using Application.Common.Interfaces;
using Domain.Events;

namespace Application.CategoryItems.Commands.DeleteCategoryItem;
public record DeleteCategoryItemCommand(int Id) : IRequest;

public class DeleteCategoryItemCommandHandler : IRequestHandler<DeleteCategoryItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCategoryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CategoryItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.CategoryItems.Remove(entity);

        entity.AddDomainEvent(new CategoryItemDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);
    }

}