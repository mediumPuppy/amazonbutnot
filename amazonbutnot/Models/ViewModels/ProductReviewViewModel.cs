using amazonbutnot.Models;
using System.Collections.Generic;

namespace amazonbutnot.Models.ViewModels
{
    public class ProductReviewViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}

