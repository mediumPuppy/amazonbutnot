using Microsoft.AspNetCore.Identity;

namespace amazonbutnot.Models;

public interface IRolesRepository
{
    Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
    Task<IdentityRole> FindByIdAsync(string roleId);
    Task<IdentityRole> FindByNameAsync(string roleName);
    Task<bool> CreateAsync(IdentityRole role);
    Task<bool> UpdateAsync(IdentityRole role);
    Task<bool> DeleteAsync(IdentityRole role);
}