using System.Reflection;
using Application.Common.Mappings;
using Application.NewsItems.Commands.CreateNewsItem;
using Application.NewsItems.Commands.UpdateNewsItem;
using AutoMapper;
using Dashboard.Pages;

namespace Dashboard.Common.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap(typeof(CreateNewsItemCommand), typeof(EditNewsItem.EditNewsItemDto)).ReverseMap();
        CreateMap(typeof(UpdateNewsItemCommand), typeof(EditNewsItem.EditNewsItemDto)).ReverseMap();
    }
}

