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

        public byte product_ID { get; set; }
        public Product Product { get; set; }

        public byte qty { get; set; }
        public byte rating { get; set; }
    }
}
