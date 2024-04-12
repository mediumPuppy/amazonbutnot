using amazonbutnot.Models;
using amazonbutnot.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.OnnxRuntime;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using System.Drawing;

namespace amazonbutnot.Controllers;

public class AdminController : Controller
{
    private readonly IRolesRepository _rolesRepository;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<Customer> _userManager;
    private IProductRepository _repo;
    private readonly InferenceSession _session;

    public AdminController(IRolesRepository rolesRepository, RoleManager<IdentityRole> roleManager,UserManager<Customer> userManager, IProductRepository temp, InferenceSession session)
    {

        _rolesRepository = rolesRepository;
        _roleManager = roleManager;
        _userManager = userManager;
        _repo = temp;
        _session = session;
    }

    [HttpGet]
    public IActionResult AdminIndex()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> UserRoles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var viewModel = new RoleManagementViewModel
        {
            Roles = roles
        };
        return View(viewModel);
    }

    public async Task<IActionResult> UpdateRole(string roleId, string newName)
    {
        // Find the role by ID
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            // Handle the case where the role doesn't exist
            return NotFound($"Role with ID '{roleId}' not found.");
        }

        // Update the role's name
        role.Name = newName;
        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            // Role updated successfully
            return RedirectToAction("UserRoles", "Admin"); // Redirect or return success response
        }

        // If we got this far, something failed, redisplay form
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("UserRoles"); // Return to the view with errors (or handle errors as appropriate)
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(string newRoleName)
    {
        if (!string.IsNullOrEmpty(newRoleName))
        {
            // Check if the role already exists
            var roleExists = await _roleManager.RoleExistsAsync(newRoleName);
            if (!roleExists)
            {
                // Create the new role
                var result = await _roleManager.CreateAsync(new IdentityRole(newRoleName));
                if (!result.Succeeded)
                {
                    // Handle errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    // You might want to pass back the view model here, adjusting as necessary
                    return View("UserRoles", new RoleManagementViewModel());
                }
            }
            else
            {
                ModelState.AddModelError("", "Role already exists.");
                return View("UserRoles", new RoleManagementViewModel());
            }
        }

        // Redirect or return as appropriate, maybe to the same page to show a success message
        return RedirectToAction("UserRoles");
    }

    public async Task<IActionResult> EditRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return NotFound($"Role with ID '{roleId}' not found.");
        }

        var viewModel = new RoleManagementViewModel
        {
            RoleId = role.Id,
            NewName = role.Name // Use NewName or a different property for the edit
        };

        return View("EditRole", viewModel); // Assuming you have an EditRole.cshtml view
    }

    public async Task<IActionResult> ConfirmDelete(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return NotFound();
        }

        var viewModel = new RoleManagementViewModel
        {
            RoleId = role.Id,
            NewName = role.Name
        };

        return View(viewModel);
    }

    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                // Optionally add a success message or log the deletion
                return RedirectToAction("UserRoles");
            }
            else
            {
                // Handle the case where deletion fails
                // You might want to return to the confirmation page with an error message
            }
        }

        return NotFound();

    }
    
    
// DELETE USER (ADMIN ROLE) BELOW-----------------------
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);  
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);  
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Successfully deleted user";
                return RedirectToAction("UserManagement"); 
            }
            else
            {
                return Content("could not delete"); 
            }
        }

        return NotFound();
    }

    // FIND EDIT USER MANAGEMENT BELOW ------------------------------
    public async Task<IActionResult> UserManagement(int pageNum = 1)
    {
        int pageSize = 10; // Number of items per page

        var totalItems = await _userManager.Users.CountAsync();

        var pagedUsers = await _userManager.Users
                                           .Skip((pageNum - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync();

        var model = new UserManagementViewModel
        {
            Users = pagedUsers,
            Pagination = new PaginationInfo
            {
                CurrentPage = pageNum,
                ItemsPerPage = pageSize,
                TotalItems = totalItems
            }
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        var model = new UserManagementViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(UserManagementViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new Customer { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Successfully added user";
                return RedirectToAction("UserManagement");
            }
            else
            {
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                
                return View(model);
            }
        }

        // If the model state is not valid, return to the view with the model to show validation errors
        return View(model);
    }
//EDIT USER 
    [HttpGet]
    public async Task<IActionResult> EditUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var model = new UserManagementViewModel { UserId = user.Id, Email = user.Email };
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> EditUser(UserManagementViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            var updateNeeded = false;

            if (user.Email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                updateNeeded = true;
            }

            if (!string.IsNullOrEmpty(model.Password) && model.Password == model.Password)
            {
                var setPasswordResult = await _userManager.ChangePasswordAsync(user, model.Password, model.Password);
                if (!setPasswordResult.Succeeded)
                {
                    foreach (var error in setPasswordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                updateNeeded = true;
            }

            if (updateNeeded)
            {
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            return RedirectToAction("UserManagement"); 
        }
        return View(model);
    }

    public async Task<IActionResult> DeleteConfirm(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var viewModel = new UserManagementViewModel
        {
            UserId = user.Id,
            Email = user.Email
        };

        return View(viewModel);
    }
    public async Task<IActionResult> MakeAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            ViewBag.ErrorMessage = "User not found.";
        }
        else if (await _userManager.IsInRoleAsync(user, "admin"))
        {
            ViewBag.ErrorMessage = "User is already an admin.";
        }
        else
        {
            var result = await _userManager.AddToRoleAsync(user, "admin");
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Successfully added user as admin.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return RedirectToAction("Results", new { message = ViewBag.ErrorMessage ?? ViewBag.SuccessMessage, alertType = ViewBag.ErrorMessage != null ? "danger" : "success" });
    }

    public async Task<IActionResult> MakeCust(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            ViewBag.ErrorMessage = "User not found.";
        }
        else if (await _userManager.IsInRoleAsync(user, "customer"))
        {
            ViewBag.ErrorMessage = "User is already a customer.";
        }
        else
        {
            var result = await _userManager.AddToRoleAsync(user, "customer");
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Successfully added user as customer.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return RedirectToAction("Results", new { message = ViewBag.ErrorMessage ?? ViewBag.SuccessMessage, alertType = ViewBag.ErrorMessage != null ? "danger" : "success" });
    }

    [HttpGet]
    public IActionResult Results(string message, string alertType)
    {
        // Clear ViewBag variables to prevent them from being retained across redirects
        ViewBag.ErrorMessage = null;
        ViewBag.SuccessMessage = null;

        ViewBag.Message = message;
        ViewBag.AlertType = alertType;
        return View();
    }






}