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


            // List of all categories
            var allCategories = new List<string>
                {
                    "Part",
                    "Structure",
                    "Energy",
                    "Harry_Potter",
                    "Flight",
                    "Minifig",
                    "Character",
                    "Disney",
                    "Colorful",
                    "Animal",
                    "Vehicle",
                    "Miscel"
                };

            return View(allCategories);
        }


    }
}
