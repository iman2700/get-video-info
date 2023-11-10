using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events;

namespace Application.CategoryItems.Commands.CreateCategoryItem;

public record CreateCategoryItemCommand : IRequest<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsChecked { get; set; }
}

public class CreateCategoryItemCommandHandler : IRequestHandler<CreateCategoryItemCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCategoryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new CategoryItem
        {
            Name = request.Name,
            Description = request.Description,
            IsChecked = request.IsChecked,
        };

        entity.AddDomainEvent(new CategoryItemCreatedEvent(entity));

        _context.CategoryItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
