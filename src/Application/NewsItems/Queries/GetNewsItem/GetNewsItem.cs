using Application.Common.Interfaces;
using Application.Common.Models;
using Application.NewsItems.Queries.GetNewsItemsWithPagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.NewsItems.Queries.GetNewsItem;

public record GetNewsItemQuery(int Id) : IRequest<NewsItemPageDto>;

public class GetNewsItemsWithPaginationQueryHandler : IRequestHandler<GetNewsItemQuery, NewsItemPageDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetNewsItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<NewsItemPageDto> Handle(GetNewsItemQuery request, CancellationToken cancellationToken)
    {
        return await _context.NewsItems
            .ProjectTo<NewsItemPageDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(n => n.Id == request.Id);
    }
}
