using Application.Common.Interfaces;
using Application.Common.Models;
using Application.NewsItems.Queries.GetNewsItemsWithPagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Tags.Queries;

public record GetTagsQuery : IRequest<List<Tag>>;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, List<Tag>>
{
    private readonly IApplicationDbContext _context;

    public GetTagsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tag>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Tags.ToListAsync(cancellationToken: cancellationToken);
    }
}
