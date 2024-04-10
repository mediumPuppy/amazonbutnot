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
    private readonly UserManager<IdentityUser> _userManager;
    private IProductRepository _repo;
    private readonly InferenceSession _session;

    public AdminController(IRolesRepository rolesRepository, RoleManager<IdentityRole> roleManager,UserManager<IdentityUser> userManager, IProductRepository temp, InferenceSession session)
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
    
    
    // Delete User Below-----------------------

    public async Task<IActionResult> DeleteUser(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                // Optionally add a success message or log the deletion
                return RedirectToAction("UserManagement");
            }
            else
            {
                // Handle the case where deletion fails
                // You might want to return to the confirmation page with an error message
            }
        }

        return NotFound();

    }
    // FIND USER MANAGEMENT BELOW ------------------------------
    public async Task<IActionResult> UserManagement()
    {
        var users = _userManager.Users;
        var model = new UserManagementViewModel { Users = users };
        return View(model);
    }
    [HttpGet]
    public IActionResult AddUser() => View();

    [HttpPost]
    public async Task<IActionResult> AddUser(UserManagementViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("UserManagement");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
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

            // Update the user's name
            // This assumes you have a method or property to update the name
            // For example, if storing the name in a claim, you would need to update the claim
            // This is a placeholder for your implementation
            
            // UpdateUserName(user, model.Name); // uncomment later jl

            // Update the email if it has changed
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
            }

            // Save changes to the user
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return RedirectToAction("UserManagement"); // Adjust as necessary
        }
        return View(model);
    }
}