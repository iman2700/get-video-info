using Application.Common.Models;
using Application.NewsItems.Queries.GetNewsItemsWithPagination;
using Application.Tags.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;


public class TagsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Tag>>> GetTags()
    {
        GetTagsQuery query = new GetTagsQuery();
        return await Mediator.Send(query);
    }
}