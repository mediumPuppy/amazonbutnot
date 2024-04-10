using Microsoft.AspNetCore.Identity;

namespace amazonbutnot.Models;
public interface IUserRepository
{
    Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
    Task<IdentityUser> FindByIdAsync(string userId);
    Task<IdentityUser> FindByNameAsync(string userName);
    Task<bool> CreateAsync(IdentityUser user);
    Task<bool> UpdateAsync(IdentityUser user);
    Task<bool> DeleteAsync(IdentityUser user);
}