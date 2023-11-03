using AutoMapper;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Articles.Queries.GetArticlesWithPagination;
public class ArticlePageDto
{
    public int Id { get; init; }
    public string Title { get; init; } = String.Empty;
    public string? Description { get; init; }
    public string? Thumbnail { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Article, ArticlePageDto>();
        }
    }
}
