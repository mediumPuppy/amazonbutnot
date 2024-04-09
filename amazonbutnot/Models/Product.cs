using System.ComponentModel.DataAnnotations;

namespace amazonbutnot.Models
{
    public class Product
    {
        [Key]
        public int product_ID { get; set; }
        public string name { get; set; }
        public int year { get; set; }
        public int num_parts { get; set; }
        public int price { get; set; }
        public string img_link { get; set; }
        public string primary_color { get; set; }
        public string secondary_color { get; set; }
        public string description { get; set; }
        public int Part { get; set; }
        public int Structure { get; set; }
        public int Energy { get; set; }
        public int HarryPotter { get; set; }
        public int Flight { get; set; }
        public int Minifig { get; set; }
        public int Character { get; set; }
        public int Disney { get; set; }
        public int Colorful { get; set; }
        public int Animal { get; set; }
        public int Vehicle { get; set; }
        public int Miscel { get; set; }
        public int rec1 { get; set; }
        public int rec2 { get; set; }
        public int rec3 { get; set; }
        public int rec4 { get; set; }
        public int rec5 { get; set; }

    }
}
