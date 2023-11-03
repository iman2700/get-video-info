using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Application.Articles.Queries.GetArticlesWithPagination;
public record GetArticlesWithPaginationQuery : IRequest<PaginatedList<ArticlePageDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetArticlesWithPaginationQuery, PaginatedList<ArticlePageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ArticlePageDto>> Handle(GetArticlesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Articles
            .OrderBy(x => x.Title)
            .ProjectTo<ArticlePageDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}