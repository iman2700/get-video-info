using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteUser;
using Application.Users.Queries.GetUserRoles;
using Application.Users.Queries.LoginUser;
using Ardalis.GuardClauses;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Controllers;

public class UsersController : ApiControllerBase
{
    private IConfiguration _configuration;
    private ILogger<UsersController> _logger;
    public UsersController(ILogger<UsersController> logger, IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string GenerateJsonWebToken(LoginUserQuery loginUserQuery, string userId, IList<string> roles)
    {
        var issuer = _configuration.GetValue<string>("Jwt:Issuer");
        var key = _configuration.GetValue<string>("Jwt:Key");
        Guard.Against.NullOrEmpty(issuer);
        Guard.Against.NullOrEmpty(key);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenClaims = new List<Claim>
        {
            new("Id", Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, loginUserQuery.Username),
            new(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };
        tokenClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var token = new JwtSecurityToken(issuer,
            issuer, tokenClaims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Microsoft.AspNetCore.Authorization.Authorize(Roles = Roles.Administrator)]
    [HttpPost]
    public async Task<ActionResult<string>> Create(CreateUserCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUserQuery loginUserQuery)
    {
        var userId = await Mediator.Send(loginUserQuery);
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                var roles = await Mediator.Send(new GetUserRolesQuery(userId));
                var stringToken = GenerateJsonWebToken(loginUserQuery, userId, roles);
                return Ok(stringToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [Microsoft.AspNetCore.Authorization.Authorize(Roles = Roles.Administrator)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Delete(string id)
    {
        await Mediator.Send(new DeleteUserCommand(id));

        return NoContent();
    }
}