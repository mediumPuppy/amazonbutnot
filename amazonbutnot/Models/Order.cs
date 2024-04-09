using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazonbutnot.Models
{
    public class Order
    {
        [Key]
        public int transaction_ID { get; set; }

        // Foreign key property
        public int customer_ID { get; set; }

        // Navigation property for the related entity
        [ForeignKey("customer_ID")]
        public Customer Customer { get; set; }

        public string date { get; set; }
        public string day_of_week { get; set; }
        public int time { get; set; }
        public string entry_mode { get; set; }
        public float amount { get; set; }
        public string type_of_transaction { get; set; }
        public int country_of_transaction { get; set; }
        public int shipping_address { get; set; }
        public string bank { get; set; }
        public string type_of_card { get; set; }
        public int fraud { get; set; }
    }
}
