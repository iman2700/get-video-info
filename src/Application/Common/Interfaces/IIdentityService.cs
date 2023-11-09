using Application.Common.Models;

namespace Application.Common.Interfaces;
public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<string> LoginAsync(string userName, string password);

    Task<IList<string>> GetRolesAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string email);

    Task<Result> DeleteUserAsync(string userId);
    Task<Result> DeleteUserByUsernameAsync(string userName);
}
