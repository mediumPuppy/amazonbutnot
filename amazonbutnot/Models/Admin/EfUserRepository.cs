using amazonbutnot.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace amazonbutnot.Models;

public class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Customer> _userManager; // Use UserManager for user operations

    public EfUserRepository(ApplicationDbContext context, UserManager<Customer> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Assuming you want to list users; if you want to list roles, this should be handled differently
    public async Task<IEnumerable<Customer>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<Customer> FindByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<Customer> FindByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<bool> CreateAsync(Customer user)
    {
        var result = await _userManager.CreateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateAsync(Customer user)
    {
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(Customer user)
    {
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}