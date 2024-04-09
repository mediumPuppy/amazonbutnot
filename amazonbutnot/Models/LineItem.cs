using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazonbutnot.Models
{
    public class LineItem
    {
        [Key, Column(Order = 1)]
        public int transaction_ID { get; set; }
        public Order Order { get; set; }

        [Key, Column(Order = 2)]
        public int product_ID { get; set; }
        public Product Product { get; set; }

        public int qty { get; set; }
        public int rating { get; set; }
    }
}
