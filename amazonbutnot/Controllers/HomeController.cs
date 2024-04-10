using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using amazonbutnot.Models;
using Microsoft.AspNetCore.Authorization;
using amazonbutnot.Models.ViewModels;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace amazonbutnot.Controllers;

public class HomeController : Controller
{
    private IProductRepository _repo;
    private readonly InferenceSession _session;

    public HomeController(IProductRepository temp, InferenceSession session)
    {
        _repo = temp;
        _session = session;
    }

    public IActionResult Index()
    {
        int pageSize = 5;

        var Blah = new ProductsListViewModel
        {
            Products = _repo.Products
        .OrderBy(product => product.name)
        .Take(pageSize),

            
        };
        return View();
    }

    [HttpPost]
    public IActionResult Predict(int time, int amount, int country_ID)
    {
        var country_isUK = 0;
        // Dictionary mapping the numeric prediction to an animal type
        var class_type_dict = new Dictionary<int, string>
            {
                { 0, "Thank you for your purchase!" },
                { 1, "Order in review. Thank you!" }
            };

        if (country_ID == 1)
        {
            country_isUK = 1; //if the country_ID is the UK, we change the country_isUK so the input is correct for when we call the model to make a prediction
        }

        try
        {
            var input = new List<float> { time, amount, country_isUK };
            var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

            var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
                };

            using (var results = _session.Run(inputs)) // makes the prediction with the inputs from the form
            {
                var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                if (prediction != null && prediction.Length > 0)
                {
                    // Use the prediction to get the animal type from the dictionary
                    var fraudStatus = class_type_dict.GetValueOrDefault((int)prediction[0], "Unknown");
                    ViewBag.Prediction = fraudStatus;
                }
                else
                {
                    ViewBag.Prediction = "Error: Unable to make a prediction.";
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.Prediction = "Error during prediction.";
        }

        return View("OrderTestPredict");
    }

    public IActionResult About()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Products(int pageNum, string? CategoryName)
    {
        int pageSize = 9;

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
                case "HarryPotter":
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


        var Blah = new ProductsListViewModel
        {
            Products = query
                .OrderBy(product => product.name)
                .Skip((pageNum) * pageSize) //I removed a -1
                .Take(pageSize),

            PaginationInfo = new PaginationInfo
            {
                CurrentPage = pageNum,
                ItemsPerPage = pageSize,
                TotalItems = query.Count()
            },

            CurrentCategory = CategoryName
        };

        return View("Products", Blah);
        
    }


    public IActionResult ProductDetails(int product_ID)
    {
        return View("ProductDetails");
    }

}