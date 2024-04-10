using Microsoft.AspNetCore.Identity;

namespace amazonbutnot.Models.ViewModels;

public class RoleManagementViewModel
{
    public IEnumerable<IdentityRole> Roles { get; set; }
    public string RoleId { get; set; }
    public string NewName { get; set; }
    public string NewRoleName { get; set; }
}