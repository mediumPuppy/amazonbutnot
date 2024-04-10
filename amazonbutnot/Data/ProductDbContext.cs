using amazonbutnot.Models;
using Microsoft.EntityFrameworkCore;

namespace amazonbutnot.Data
{
    public class ProductDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ProductDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use the "ProductDbConnection" connection string from appsettings.json
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ProductDbConnection"));
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<LineItem> LineItems { get; set; }
    }

}
