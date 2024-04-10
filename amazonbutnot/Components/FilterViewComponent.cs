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
            var selectedCategory = RouteData?.Values["CategoryName"]?.ToString();
            var selectedColor = Request.Query["Color"].ToString();

            var primaryColors = _prodRepository.Products
                .Select(x => x.primary_color)
                .Distinct()
                .OrderBy(x => x);

            var secondaryColors = _prodRepository.Products
                .Select(x => x.secondary_color)
                .Distinct()
                .OrderBy(x => x);

            var viewModel = new Filter
            {
                Categories = new List<string>
            {
                "Part",
                "Structure",
                "Energy",
                "Harry Potter",
                "Flight",
                "Minifig",
                "Character",
                "Disney",
                "Colorful",
                "Animal",
                "Vehicle",
                "Miscel"
            },
                Colors = primaryColors.Union(secondaryColors),
                
               
            };

            return View(viewModel);
        }




    }
}
