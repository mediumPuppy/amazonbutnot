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
    public IActionResult Index(int pageNum, int CategoryId)
    {
        int pageSize = 5;

        var Blah = new ProductsListViewModel
        {
            Products = _repo.Products
            .Where(x => CategoryId == 0 || x.CategoryId == CategoryId) // Check if CategoryId is 0 or matches the provided CategoryId
            .OrderBy(x => x.ProductName)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize),


            PaginationInfo = new PaginationInfo
            {
                CurrentPage = pageNum,
                ItemsPerPage = pageSize,
                TotalItems = CategoryId == 0 ? _repo.Products.Count() : _repo.Products.Where(x => x.CategoryId == CategoryId).Count()
            },

            CurrentCategoryId = CategoryId
        };

        return View("Index", Blah);
    }

}