using Microsoft.AspNetCore.Identity;

namespace amazonbutnot.Models;
public interface IUserRepository
{
    Task<IEnumerable<AspNetUser>> GetAllUsersAsync();
    Task<AspNetUser> FindByIdAsync(string userId);
    Task<AspNetUser> FindByNameAsync(string userName);
    Task<bool> CreateAsync(AspNetUser user);
    Task<bool> UpdateAsync(AspNetUser user);
    Task<bool> DeleteAsync(AspNetUser user);
}