using Microsoft.AspNetCore.Mvc;
using amazonbutnot.Models;
using System;

namespace amazonbutnot.Components
{
    public class FilterViewComponent : ViewComponent
    {
        private IProductRepository _prodRepository;

        public FilterViewComponent(IProductRepository temp)
        {
            _prodRepository = temp;
        }

 
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategories = RouteData?.Values["CategoryName"];


            var distinctCategories = _prodRepository.Products
                .Select(product => product.Category.CategoryName)
                .Distinct();
                

            return View(distinctCategories);
        }

    }
}
