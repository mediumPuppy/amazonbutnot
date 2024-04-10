using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace amazonbutnot.Models
{
    public class Customer
    {
        [Key]
        public short customer_ID { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set;}
        public DateTime birth_date { get; set; }

        // Foreign key property
        public byte country_ID { get; set; }

        // Navigation property for the related entity
        [ForeignKey("country_ID")]
        public Country Country { get; set; }
        public string gender { get; set; }
        public byte age { get; set; }
        public byte Rec1 { get; set; }
        public byte Rec2 { get; set; }
        public byte Rec3 { get; set; }
        public byte Rec4 { get; set; }
        public byte Rec5 { get; set; }
    }
}
