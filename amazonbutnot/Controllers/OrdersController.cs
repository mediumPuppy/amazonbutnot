using amazonbutnot.Models.ViewModels;
using amazonbutnot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using Azure;

namespace amazonbutnot.Controllers
{
    public class OrdersController : Controller
    {
        private IProductRepository _repo;
        private readonly InferenceSession _session;

        public OrdersController(IProductRepository temp, InferenceSession session)
        {
            _repo = temp;
            _session = session;
        }

        public IActionResult OrderReview(int pageNum = 1)
        {
            int pageSize = 25;
            var query = _repo.Orders.Where(o => o.fraud == 1);

            var ordersListViewModel = new OrderReviewViewModel
            {
                Orders = query
                    .OrderBy(order => order.transaction_ID)
                    .Skip(pageNum <= 1 ? 0 : (pageNum - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),

                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = query.Count()
                }
            };

            return View(ordersListViewModel);
        }

        public IActionResult ProductReview(int pageNum = 1)
        {
            int pageSize = 12;
            var query = _repo.Products;

            var productReviewViewModel = new ProductReviewViewModel
            {
                Products = query
                    .OrderByDescending(product => product.product_ID)
                    .Skip(pageNum <= 1 ? 0 : (pageNum - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),

                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = query.Count()
                }
            };

            return View(productReviewViewModel);
        }

        public IActionResult AddProduct()
        {
            int maxProductId = _repo.Products.Max(p => p.product_ID);
            int newProductId = maxProductId + 1;

            ViewBag.NewProductId = newProductId;

            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _repo.AddProduct(product);
                return RedirectToAction("ProductReview");
            }
            return RedirectToAction("OrderReview");
        }

        public IActionResult DeleteProduct(int product_ID)
        {
            var product = _repo.Products.FirstOrDefault(x => x.product_ID == product_ID);
            if (product == null)
            {
                return RedirectToAction("OrderReview");
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult DeleteProductConfirmed(Product product)
        {
            if (ModelState.IsValid)
            {
                _repo.DeleteProduct(product);
                return RedirectToAction("ProductReview");
            }
            return RedirectToAction("OrderReview");
        }

        public IActionResult EditProduct(int id)
        {
            var product = _repo.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _repo.UpdateProduct(product);
                return RedirectToAction("ProductReview");
            }
            return View(product);
        }
    }
}
