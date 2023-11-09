using Application.Articles.Commands.CreateArticle;
using Application.Articles.Commands.DeleteArticle;
using Application.Articles.Commands.UpdateArticle;
using Application.Articles.Queries.GetArticlesWithPagination;
using Application.Common.Models;
using Application.Common.Security;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize(Roles = Roles.Administrator)]
public class ArticlesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ArticlePageDto>>> GetArticlesWithPagination([FromQuery] GetArticlesWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateArticleCommand command)
    {
        var currentUser = HttpContext.User;
        int count = currentUser.Claims.Count();
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(int id, UpdateArticleCommand command)
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
        await Mediator.Send(new DeleteArticleCommand(id));

        return NoContent();
    }
}
