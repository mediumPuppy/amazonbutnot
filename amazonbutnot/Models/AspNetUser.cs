using Microsoft.AspNetCore.Identity;

namespace amazonbutnot.Models
{
    public class AspNetUser : IdentityUser
    {
        public string first_name { get; set; }
        public string last_name { get; set;}
        public DateTime birth_date { get; set; }
        public int country_ID { get; set; }
        public string gender { get; set; }
        public byte age { get; set; }
        public byte? Rec1 { get; set; }
        public byte? Rec2 { get; set; }
        public byte? Rec3 { get; set; }
        public byte? Rec4 { get; set; }
        public byte? Rec5 { get; set; }
    }
}
