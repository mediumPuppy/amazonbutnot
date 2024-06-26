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
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;


namespace amazonbutnot.Controllers;

public class HomeController : Controller
{
    private IProductRepository _repo;
    private readonly InferenceSession _session;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(IProductRepository temp, InferenceSession session, IHttpContextAccessor httpContextAccessor)
    {
        _repo = temp;
        _session = session;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {

        var username = User.Identity.Name;

        // Retrieve the corresponding customer from the repository
        var customer = _repo.AspNetUsers.FirstOrDefault(x => x.UserName == username);

        if (customer == null || customer.Rec1 == null) 
        {
            var blah = new ProductsListViewModel
            {
                Products = _repo.Products
                .Where(x => new[] { 27, 33, 34 }.Contains(x.product_ID))
            };
            return View(blah);
        }
        else
        {
            var blah = new ProductsListViewModel
            {
                Products = _repo.Products
                .Where(x => new[] { (int)customer.Rec1, (int)customer.Rec2, (int)customer.Rec3}.Contains(x.product_ID))
            };
            return View(blah);
        }


        
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



    public IActionResult ProductDetails(int id)
    {
        // Fetch the selected product
        var selectedProduct = _repo.Products.FirstOrDefault(p => p.product_ID == id);

        if (selectedProduct == null)
        {
            // Handle the case where the product is not found
            return NotFound();
        }

        // Fetch the recommended products
        var recommendedProducts = _repo.Products
            .Where(p => new[] { selectedProduct.rec1, selectedProduct.rec2, selectedProduct.rec3 }.Contains(p.product_ID))
            .ToList();

        // Create a view model to hold the selected product and recommended products
        var viewModel = new ProductDetailsViewModel
        {
            SelectedProduct = selectedProduct,
            RecommendedProducts = recommendedProducts
        };

        return View(viewModel);
    }

    //[HttpGet]
    //public IActionResult OrderTestPredict()
    //{
    //    return View();
    //}

    [HttpPost]
    public IActionResult Predict(int amount)
    {
        // Check if the user is authenticated
        if (!User.Identity.IsAuthenticated)
        {
            // Redirect to the login page
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var country_isUK = 0;
        // Dictionary mapping the numeric prediction to a response message based on if its suspected fraud
        var class_type_dict = new Dictionary<int, string>
        {
            { 0, "Thank you for your purchase!" },
            { 1, "Order in review. Thank you!" }
        };

        // Get the username of the current logged-in user
        var username = User.Identity.Name;

        // Retrieve the corresponding customer from the repository
        var customer = _repo.AspNetUsers.FirstOrDefault(x => x.UserName == username);

        // Access the country_ID attribute of the customer and store it in a variable
        var country_ID = customer.country_ID;

        var custID = customer.Id;
        

        var time = DateTime.Now.Hour;

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
                    ViewBag.Prediction = "Thank you.";
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.Prediction = "Thank you!";
        }

        // Clear the cart after the checkout process
        _httpContextAccessor.HttpContext.Session.Clear();


        //Update the orders database
        int transaction_ID = _repo.Orders.Max(t => t.transaction_ID);
        DateTime date = DateTime.Today;
        string day_of_week = date.DayOfWeek.ToString();
        byte timeByte = (byte)time;
        string entry_mode = "CVC";
        double OrderAmount = (double)amount;
        byte shipping_address = (byte)country_ID;
        string bank = "Barclays";
        string type_of_card = "Visa";
        int fraud;
        fraud = (ViewBag.Prediction == "Order in review. Thank you!") ? 1 : 0;

        Order newOrder = new Order
        {
            transaction_ID = transaction_ID + 1,
            customer_ID = custID, // Assuming you have the customer instance available
            date = date,
            day_of_week = day_of_week,
            time = timeByte,
            entry_mode = entry_mode,
            amount = OrderAmount,
            type_of_transaction = "Online",
            country_of_transaction = (byte)country_ID,
            shipping_address = (byte)country_ID, // Assuming country_ID corresponds to the shipping address
            bank = "Barclays",
            type_of_card = "Visa",
            fraud = fraud 
        };

        // Add the new order to the repository and save the changes
        _repo.AddOrder(newOrder);



        return View("Checkout");
    }

}