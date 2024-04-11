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
            int pageSize = 10;
            var query = _repo.Products;

            var productReviewViewModel = new ProductReviewViewModel
            {
                Products = query
                    .OrderBy(product => product.product_ID)
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

        public IActionResult DeleteProduct(Product product)
        {
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult DeleteProductConfirmed(Product product)
        {
            if (ModelState.IsValid)
            {
                _repo.DeleteProduct(product.product_ID);
                return RedirectToAction("ProductReview");
            }
            return RedirectToAction("OrderReview");
        }




        //public IActionResult UpdateProduct(int id)
        //{
        //    var product = _repo.Products.Where(x => x.product_ID == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //[HttpPost]
        //public IActionResult UpdateProduct(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _repo.UpdateProduct(product);
        //        return RedirectToAction("ProductReview");
        //    }
        //    return View(product);
        //}

    }
}
