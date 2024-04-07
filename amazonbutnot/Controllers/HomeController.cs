using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using amazonbutnot.Models;
using Microsoft.AspNetCore.Authorization;
using amazonbutnot.Models.ViewModels;

namespace amazonbutnot.Controllers;

public class HomeController : Controller
{
    private IProductRepository _repo;

    public HomeController(IProductRepository temp)
    {
        _repo = temp;
    }

    [Authorize]
    public IActionResult Index(int pageNum, string? CategoryName)
    {
        int pageSize = 5;

        var Blah = new ProductsListViewModel
        {
            Products = _repo.Products
        .Where(product => string.IsNullOrEmpty(CategoryName) || product.Category.CategoryName == CategoryName)
        .OrderBy(product => product.ProductName)
        .Skip((pageNum - 1) * pageSize)
        .Take(pageSize),

            PaginationInfo = new PaginationInfo
            {
                CurrentPage = pageNum,
                ItemsPerPage = pageSize,
                TotalItems = string.IsNullOrEmpty(CategoryName) ? _repo.Products.Count() : _repo.Products.Count(product => product.Category.CategoryName == CategoryName)
            },

            CurrentCategory = CategoryName
        };


        return View("Index", Blah);
    }
    public IActionResult Jefferson()
    {
        return View("TableTest");
    }

}