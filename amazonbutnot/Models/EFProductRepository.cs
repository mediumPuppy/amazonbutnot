
using amazonbutnot.Data;

namespace amazonbutnot.Models
{
    public class EFProductRepository : IProductRepository
    {
        private ProductDbContext _context;
        public EFProductRepository(ProductDbContext temp)
        {
            _context = temp;
        }
        public IQueryable<Product> Products => _context.Products;

        public IQueryable<Customer> Customers => _context.Customers;

        public IQueryable<Order> Orders => _context.Orders;

        public IQueryable<Country> Countries => _context.Countries;

        public IQueryable<LineItem> LineItems => _context.LineItems;
    }
}
