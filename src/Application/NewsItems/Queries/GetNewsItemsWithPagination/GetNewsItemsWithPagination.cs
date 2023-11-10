using Application.Common.Models;
using Application.Common.Interfaces;
using Application.Common.Mappings;

namespace Application.NewsItems.Queries.GetNewsItemsWithPagination;
public record GetNewsItemsWithPaginationQuery : IRequest<PaginatedList<NewsItemPageDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetNewsItemsWithPaginationQueryHandler : IRequestHandler<GetNewsItemsWithPaginationQuery, PaginatedList<NewsItemPageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetNewsItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<NewsItemPageDto>> Handle(GetNewsItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.NewsItems
            .OrderBy(x => x.Title)
            .ProjectTo<NewsItemPageDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}