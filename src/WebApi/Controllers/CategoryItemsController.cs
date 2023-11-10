using Application.Common.Models;
using Application.CategoryItems.Commands.CreateCategoryItem;
using Application.CategoryItems.Commands.DeleteCategoryItem;
using Application.CategoryItems.Commands.UpdateCategoryItem;
using Application.CategoryItems.Queries.GetCategoryItemsWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class CategoryItemsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<CategoryItemPageDto>>> GetCategoryItemsWithPagination([FromQuery] GetCategoryItemsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateCategoryItemCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(int id, UpdateCategoryItemCommand command)
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
        await Mediator.Send(new DeleteCategoryItemCommand(id));

        return NoContent();
    }
}