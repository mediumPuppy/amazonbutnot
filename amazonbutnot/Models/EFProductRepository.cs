
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

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(Product product)
        {
            var existingProduct = _context.Products.FirstOrDefault(p => p.product_ID == product.product_ID);
            if (existingProduct != null)
            {
                _context.Products.Remove(existingProduct);
                _context.SaveChanges();
            }
        }

        public Product GetProductById(int productId)
        {
            return _context.Products.Find(productId);
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public IQueryable<Customer> Customers => _context.Customers;

        public IQueryable<Order> Orders => _context.Orders;

        public IQueryable<Country> Countries => _context.Countries;

        public IQueryable<LineItem> LineItems => _context.LineItems;
    }
}
