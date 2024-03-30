
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
    }
}
