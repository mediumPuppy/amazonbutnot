using System.ComponentModel.DataAnnotations;

namespace amazonbutnot.Models
{
    public class Country
    {
        [Key]
        public int country_ID { get; set; }
        public string country_name { get; set; }
    }
}
