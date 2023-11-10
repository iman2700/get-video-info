using Application.NewsItems.Commands.CreateNewsItem;
using Application.NewsItems.Commands.DeleteNewsItem;
using Application.NewsItems.Commands.UpdateNewsItem;
using Application.NewsItems.Queries.GetNewsItemsWithPagination;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

//[Microsoft.AspNetCore.Authorization.Authorize(Roles = Roles.Administrator)]
public class NewsItemsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<NewsItemPageDto>>> GetNewsItemsWithPagination([FromQuery] GetNewsItemsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateNewsItemCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(int id, UpdateNewsItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteNewsItemCommand(id));

        return NoContent();
    }
}
