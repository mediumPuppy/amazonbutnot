using System.ComponentModel.DataAnnotations;

namespace amazonbutnot.Models
{
    public class Product
    {
        [Key]
        public int product_ID { get; set; }
        public string name { get; set; }
        public short year { get; set; }
        public short num_parts { get; set; }
        public short price { get; set; }
        public string img_link { get; set; }
        public string primary_color { get; set; }
        public string secondary_color { get; set; }
        public string description { get; set; }
        public byte Part { get; set; }
        public byte Structure { get; set; }
        public byte Energy { get; set; }
        public byte Harry_Potter { get; set; }
        public byte Flight { get; set; }
        public byte Minifig { get; set; }
        public byte Character { get; set; }
        public byte Disney { get; set; }
        public byte Colorful { get; set; }
        public byte Animal { get; set; }
        public byte Vehicle { get; set; }
        public byte Miscel { get; set; }
        public int rec1 { get; set; }
        public int rec2 { get; set; }
        public int rec3 { get; set; }
        public int rec4 { get; set; }
        public int rec5 { get; set; }

    }
}
