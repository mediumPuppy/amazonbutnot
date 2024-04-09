using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazonbutnot.Models
{
    public class Customer
    {
        [Key]
        public int customer_ID { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set;}
        public string birth_date { get; set; }

        // Foreign key property
        public int country_ID { get; set; }

        // Navigation property for the related entity
        [ForeignKey("country_ID")]
        public Country Country { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public int Rec1 { get; set; }
        public int Rec2 { get; set; }
        public int Rec3 { get; set; }
        public int Rec4 { get; set; }
        public int Rec5 { get; set; }
    }
}
