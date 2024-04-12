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
        if (!User.IsInRole("admin"))
        {
            // Redirect to a specific route or page for unauthorized access
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        return View();
    }
    
    
// DELETE USER (ADMIN ROLE) BELOW-----------------------
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        if (!User.IsInRole("admin"))
        {
            // Redirect to a specific route or page for unauthorized access
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
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
        if (!User.IsInRole("admin"))
        {
            // Redirect to a specific route or page for unauthorized access
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
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
        if (!User.IsInRole("admin"))
        {
            // Redirect to a specific route or page for unauthorized access
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
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
        if (!User.IsInRole("admin"))
        {
            // Redirect to a specific route or page for unauthorized access
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
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