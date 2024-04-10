using amazonbutnot.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace amazonbutnot.Models;

public class EfRolesRepository : IRolesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public EfRolesRepository(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<IdentityRole> FindByIdAsync(string roleId)
    {
        return await _roleManager.FindByIdAsync(roleId);
    }

    public async Task<IdentityRole> FindByNameAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task<bool> CreateAsync(IdentityRole role)
    {
        var result = await _roleManager.CreateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> UpdateAsync(IdentityRole role)
    {
        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(IdentityRole role)
    {
        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }
}