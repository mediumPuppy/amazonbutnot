using amazonbutnot.Models;
using amazonbutnot.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace amazonbutnot.Controllers;

public class AdminController : Controller
{
    private readonly IRolesRepository _rolesRepository;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(IRolesRepository rolesRepository, RoleManager<IdentityRole> roleManager)
    {

        _rolesRepository = rolesRepository;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult AdminIndex()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> AdminUsers()
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
            return RedirectToAction("AdminUsers", "Admin"); // Redirect or return success response
        }

        // If we got this far, something failed, redisplay form
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("AdminUsers"); // Return to the view with errors (or handle errors as appropriate)
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
                    return View("AdminUsers", new RoleManagementViewModel());
                }
            }
            else
            {
                ModelState.AddModelError("", "Role already exists.");
                return View("AdminUsers", new RoleManagementViewModel());
            }
        }

        // Redirect or return as appropriate, maybe to the same page to show a success message
        return RedirectToAction("AdminUsers");
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
                return RedirectToAction("AdminUsers");
            }
            else
            {
                // Handle the case where deletion fails
                // You might want to return to the confirmation page with an error message
            }
        }

        return NotFound();

    }
}