namespace amazonbutnot.Models
{
    public class AspNetUser
    {
        public string Id { get; set; }
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
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email {  get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
    }
}
