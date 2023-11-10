using Application.Common.Interfaces;

namespace Application.CategoryItems.Commands.UpdateCategoryItem;
public class UpdateCategoryItemCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsChecked { get; set; }
}

public class UpdateCategoryItemCommandHandler : IRequestHandler<UpdateCategoryItemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateCategoryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateCategoryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CategoryItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.IsChecked = request.IsChecked;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
