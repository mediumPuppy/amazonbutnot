namespace amazonbutnot.Models.ViewModels;

public class UserManagementViewModel
{
    public IEnumerable<AspNetUser> Users { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public PaginationInfo Pagination { get; set; }

}