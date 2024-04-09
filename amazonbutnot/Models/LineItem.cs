using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace amazonbutnot.Models
{
    [Keyless]
    public class LineItem
    {
        public int transaction_ID { get; set; }
        public Order Order { get; set; }

        public int product_ID { get; set; }
        public Product Product { get; set; }

        public int qty { get; set; }
        public int rating { get; set; }
    }
}
