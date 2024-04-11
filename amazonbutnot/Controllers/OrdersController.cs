using amazonbutnot.Models.ViewModels;
using amazonbutnot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;

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




    }
}
