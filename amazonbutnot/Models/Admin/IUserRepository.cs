using Microsoft.AspNetCore.Identity;

namespace amazonbutnot.Models;
public interface IUserRepository
{
    Task<IEnumerable<Customer>> GetAllUsersAsync();
    Task<Customer> FindByIdAsync(string userId);
    Task<Customer> FindByNameAsync(string userName);
    Task<bool> CreateAsync(Customer user);
    Task<bool> UpdateAsync(Customer user);
    Task<bool> DeleteAsync(Customer user);
}