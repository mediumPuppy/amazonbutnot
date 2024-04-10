using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using amazonbutnot.Models;
using Microsoft.AspNetCore.Authorization;
using amazonbutnot.Models.ViewModels;
using System.Drawing.Printing;

namespace amazonbutnot.Controllers;

public class HomeController : Controller
{
    private IProductRepository _repo;

    public HomeController(IProductRepository temp)
    {
        _repo = temp;
    }

    public IActionResult Index()
    {

        var blah = new ProductsListViewModel
        {
            Products = _repo.Products
                .Where(x => new[] { 27, 33, 34, 37, 24 }.Contains(x.product_ID))
        };
        return View(blah);
    }
    
    public IActionResult About()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Products(int pageNum, string? CategoryName, string? Color, int PageSize = 5)
    {
        int pageSize = PageSize;
        var query = _repo.Products.AsQueryable();

        // Apply filtering based on the selected category attribute
        if (!string.IsNullOrEmpty(CategoryName))
        {
            switch (CategoryName)
            {
                case "Part":
                    query = query.Where(product => product.Part == 1);
                    break;
                case "Structure":
                    query = query.Where(product => product.Structure == 1);
                    break;
                case "Energy":
                    query = query.Where(product => product.Energy == 1);
                    break;
                case "Harry Potter":
                    query = query.Where(product => product.Harry_Potter == 1);
                    break;
                case "Flight":
                    query = query.Where(product => product.Flight == 1);
                    break;
                case "Minifig":
                    query = query.Where(product => product.Minifig == 1);
                    break;
                case "Character":
                    query = query.Where(product => product.Character == 1);
                    break;
                case "Disney":
                    query = query.Where(product => product.Disney == 1);
                    break;
                case "Colorful":
                    query = query.Where(product => product.Colorful == 1);
                    break;
                case "Animal":
                    query = query.Where(product => product.Animal == 1);
                    break;
                case "Vehicle":
                    query = query.Where(product => product.Vehicle == 1);
                    break;
                case "Miscel":
                    query = query.Where(product => product.Miscel == 1);
                    break;
                default:
                    // Handle unknown category names or provide a default filter
                    break;
            }
        }

        // Apply filtering based on the selected color
        if (!string.IsNullOrEmpty(Color))
        {
            query = query.Where(product => product.primary_color == Color || product.secondary_color == Color);
        }

        var Blah = new ProductsListViewModel
        {
            Products = query
                .OrderBy(product => product.name)
                .Skip(pageNum <= 1 ? 0 : (pageNum - 1) * pageSize)
                .Take(pageSize),

            PaginationInfo = new PaginationInfo
            {
                CurrentPage = pageNum,
                ItemsPerPage = pageSize,
                TotalItems = query.Count()
            },

            CurrentCategory = CategoryName,
            SelectedColor = Color,
            SelectedPageSize = pageSize

        };

        return View("Products", Blah);
    }



    public IActionResult ProductDetails(int product_ID)
    {
        return View("ProductDetails");
    }

}