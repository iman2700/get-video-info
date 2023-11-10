using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;

namespace Application.CategoryItems.Queries.GetCategoryItemsWithPagination;
public record GetCategoryItemsWithPaginationQuery : IRequest<PaginatedList<CategoryItemPageDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCategoryItemsWithPaginationQueryHandler : IRequestHandler<GetCategoryItemsWithPaginationQuery, PaginatedList<CategoryItemPageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoryItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CategoryItemPageDto>> Handle(GetCategoryItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.CategoryItems
            .OrderBy(x => x.Name)
            .ProjectTo<CategoryItemPageDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}