using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazonbutnot.Models
{
    public class Order
    {
        [Key]
        public int transaction_ID { get; set; }

        // Foreign key property
        public short customer_ID { get; set; }

        // // Navigation property for the related entity
        // [ForeignKey("customer_ID")]
        // public Customer Customer { get; set; }

        public DateTime date { get; set; }
        public string day_of_week { get; set; }
        public byte time { get; set; }
        public string entry_mode { get; set; }
        public double amount { get; set; }
        public string type_of_transaction { get; set; }
        public byte country_of_transaction { get; set; }
        public byte shipping_address { get; set; }
        public string bank { get; set; }
        public string type_of_card { get; set; }
        public int fraud { get; set; }
    }
}
