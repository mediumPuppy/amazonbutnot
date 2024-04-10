using Microsoft.AspNetCore.Identity;

namespace amazonbutnot.Models.ViewModels;

public class UserManagementViewModel
{
    public IEnumerable<IdentityUser> Users { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }}