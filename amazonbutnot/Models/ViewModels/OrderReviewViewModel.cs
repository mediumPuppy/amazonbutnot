using System.Collections.Generic;

namespace amazonbutnot.Models.ViewModels
{
    public class OrderReviewViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}

