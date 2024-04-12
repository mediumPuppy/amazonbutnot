using amazonbutnot.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace amazonbutnot.Models;

public class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AspNetUser> _userManager; // Use UserManager for user operations

    public EfUserRepository(ApplicationDbContext context, UserManager<AspNetUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Assuming you want to list users; if you want to list roles, this should be handled differently
    public async Task<IEnumerable<AspNetUser>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<AspNetUser> FindByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<AspNetUser> FindByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<bool> CreateAsync(AspNetUser user)
    {
        var result = await _userManager.CreateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateAsync(AspNetUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(AspNetUser user)
    {
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}